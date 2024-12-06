using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace XRC.Assignments.Project.G09
{
    public class ColorPanel : MonoBehaviour
    {
        [SerializeField] private GameObject m_ColorPanel;
        [SerializeField] private RawImage m_ColorWheel;
        [SerializeField] private float m_CylinderHeight = 100f;

        private float m_Hue = 0f;
        private float m_Saturation = 0f;
        private float m_Value = 1f;

        private bool isChanging = false;
        private Color m_Color;

        private XRBaseInteractor m_Interactor;

        public Color color => m_Color;

        void Start()
        {
            m_ColorPanel.SetActive(false);
            m_Interactor = GetComponent<ChangeColorLogic>().interactor;

            BuildColorWheelTexture();
        }

        void Update()
        {
            if (isChanging)
            {
                PickColor();

                m_Color = Color.HSVToRGB(m_Hue, m_Saturation, m_Value);
            }
        }

        public void SetActive(bool active)
        {
            // Set the panel at the position of interactor
            m_ColorPanel.transform.position = m_Interactor.transform.position;

            // Face camera
            Vector3 direction = Camera.main.transform.position - m_ColorPanel.transform.position;
            direction.y = 0f;
            m_ColorPanel.transform.rotation = Quaternion.LookRotation(-direction);

            m_ColorPanel.SetActive(active);
            isChanging = active;
        }

        private void PickColor()
        {
            Vector3 pointerPos = m_ColorPanel.transform.InverseTransformPoint(m_Interactor.transform.position);
            Vector3 diff = pointerPos;

            float angle = Mathf.Atan2(pointerPos.y, pointerPos.x);
            if (angle < 0)
            {
                angle += Mathf.PI * 2f;
            }

            m_Hue = Mathf.Clamp01(angle / (Mathf.PI * 2f));

            Vector2 distanceXY = new Vector2(diff.x, diff.y);
            m_Saturation = Mathf.Clamp01(distanceXY.magnitude / 100f);

            // Set V and move color wheel
            float posZ = Mathf.Clamp(diff.z, -m_CylinderHeight, 0f);
            m_ColorWheel.transform.localPosition = new Vector3(0, 0, posZ);
            m_Value = -posZ / m_CylinderHeight;
            m_ColorWheel.color = Color.HSVToRGB(0, 0, m_Value);
        }

        private void BuildColorWheelTexture()
        {
            Canvas canvas = m_ColorWheel.GetComponent<Canvas>();
            Rect rect = RectTransformUtility.PixelAdjustRect(m_ColorWheel.rectTransform, canvas);

            int wheelTextureSize = 5 * Mathf.FloorToInt(Mathf.Min(rect.width, rect.height));

            Texture2D wheelTexture = new Texture2D(wheelTextureSize, wheelTextureSize, TextureFormat.RGBA32, false);

            float maxDistance = (wheelTextureSize / 2f);

            for (int y = 0; y < wheelTextureSize; y++)
            {
                for (int x = 0; x < wheelTextureSize; x++)
                {
                    Vector2 vectorFromCentre = new Vector2(x - (wheelTextureSize / 2f),
                        y - (wheelTextureSize / 2f));

                    float distanceFromCenter = vectorFromCentre.sqrMagnitude;

                    if (vectorFromCentre.sqrMagnitude < maxDistance * maxDistance)
                    {
                        float angle = Mathf.Atan2(vectorFromCentre.y, vectorFromCentre.x);
                        if (angle < 0)
                        {
                            angle += Mathf.PI * 2f;
                        }

                        float hue = Mathf.Clamp01(angle / (Mathf.PI * 2f));
                        float saturation = Mathf.Clamp01(Mathf.Sqrt(distanceFromCenter) / maxDistance);

                        wheelTexture.SetPixel(x, y, Color.HSVToRGB(hue, saturation, 1f));
                    }
                    else
                    {
                        wheelTexture.SetPixel(x, y, Color.clear);
                    }
                }
            }

            wheelTexture.Apply();
            m_ColorWheel.texture = wheelTexture;
        }
    }
}