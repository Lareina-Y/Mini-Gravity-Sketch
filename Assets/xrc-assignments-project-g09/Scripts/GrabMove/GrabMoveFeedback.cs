using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace XRC.Assignments.Project.G09
{
    /// <summary>
    /// Generate feedback for the grab move technique
    /// by rendering a line between the controllers and a sphere as pivot.
    /// </summary>
    public class GrabMoveFeedback : MonoBehaviour
    {
        [SerializeField] Color m_PivotColor;
        [SerializeField] Color m_LineColor;

        private Transform m_LeftTransform;
        private Transform m_RightTransform;

        private GameObject m_PivotSphere;
        private LineRenderer m_LineRenderer;
        private MeshRenderer m_SphereRenderer;
        private TextMeshPro m_ScaleText;

        private float m_CurrScale = 1.0f;

        private float m_PivotScale = 0.006f;
        private float m_LineWidth = 0.003f;

        void Awake()
        {
            m_LeftTransform = GetComponent<GrabMoveLogic>().leftTransform;
            m_RightTransform = GetComponent<GrabMoveLogic>().rightTransform;
        }

        void Start()
        {
            SetUpVisualization();
        }

        void Update()
        {
            if (GetComponent<GrabMoveLogic>().isGrabMoving)
            {
                StartVisualization();
                TriggerVibrationFeedback();
            }
            else
            {
                ClearVisualization();
            }
        }

        private void SetUpVisualization()
        {
            // Set up pivot sphere
            m_PivotSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_PivotSphere.transform.SetParent(m_RightTransform);
            m_PivotSphere.GetComponent<SphereCollider>().enabled = false;
            m_PivotSphere.transform.localScale = Vector3.one * m_PivotScale;

            m_SphereRenderer = m_PivotSphere.GetComponent<MeshRenderer>();
            m_SphereRenderer.sortingOrder = 1;
            m_SphereRenderer.material = new Material(Shader.Find("Sprites/Default"));
            m_SphereRenderer.material.color = m_PivotColor;

            // Set up line renderer
            m_LineRenderer = m_PivotSphere.AddComponent<LineRenderer>();
            m_LineRenderer.sortingOrder = 0;
            m_LineRenderer.positionCount = 2;
            m_LineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            m_LineRenderer.startColor = m_LineColor;
            m_LineRenderer.endColor = m_LineColor;
            m_LineRenderer.startWidth = m_LineWidth;
            m_LineRenderer.endWidth = m_LineWidth;
            m_LineRenderer.useWorldSpace = true;

            // Set up text
            GameObject text = new GameObject("Percentage");
            text.transform.SetParent(m_SphereRenderer.transform);
            text.transform.localScale = Vector3.one;

            m_ScaleText = text.AddComponent<TextMeshPro>();
            m_ScaleText.renderer.sortingOrder = 2;
            m_ScaleText.color = Color.white;
            m_ScaleText.fontSize = 20;
            m_ScaleText.alignment = TextAlignmentOptions.Center;
        }

        private void StartVisualization()
        {
            m_CurrScale = GetComponent<GrabMoveLogic>().currentScale;

            // Update line
            m_LineRenderer.enabled = true;
            m_LineRenderer.startWidth = m_LineWidth * m_CurrScale;
            m_LineRenderer.endWidth = m_LineWidth * m_CurrScale;
            m_LineRenderer.SetPosition(0, m_LeftTransform.position);
            m_LineRenderer.SetPosition(1, m_RightTransform.position);

            // Update sphere (scale following XROrigin)
            m_SphereRenderer.enabled = true;
            Vector3 pivot = (m_RightTransform.position + m_LeftTransform.position) / 2;
            m_PivotSphere.transform.position = pivot;

            // Update text
            m_ScaleText.enabled = true;
            float percentage = 100f / m_CurrScale;
            m_ScaleText.transform.position = pivot;
            m_ScaleText.transform.rotation = Camera.main.transform.rotation;
            m_ScaleText.text = percentage.ToString("0.") + "%";
        }

        private void ClearVisualization()
        {
            m_LineRenderer.enabled = false;
            m_SphereRenderer.enabled = false;
            m_ScaleText.enabled = false;
        }

        private void TriggerVibrationFeedback()
        {
            float amplitude = Mathf.Min(1f / m_CurrScale, 1.0f);

            m_RightTransform.gameObject.GetComponent<HapticImpulsePlayer>().SendHapticImpulse(amplitude, 0.01f);
            m_LeftTransform.gameObject.GetComponent<HapticImpulsePlayer>().SendHapticImpulse(amplitude, 0.01f);
        }
    }
}
