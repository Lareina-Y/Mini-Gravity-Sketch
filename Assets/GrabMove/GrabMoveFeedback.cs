using TMPro;
using UnityEngine;

public class GrabMoveFeedback : MonoBehaviour
{
    [SerializeField] Color m_PivotColor;
    [SerializeField] Color m_LineColor;
    
    private Transform m_LeftTransform;
    private Transform m_RightTransform;
    
    private LineRenderer m_LineRenderer;
    private TextMeshPro m_ScaleText;

    void Awake()
    {
        m_LeftTransform = GetComponent<GrabMoveLogic>().leftTransform;
        m_RightTransform = GetComponent<GrabMoveLogic>().rightTransform;
    }

    void Start()
    {
        SetUpVisualization();
    }

    void LateUpdate()
    {
        if (GetComponent<GrabMoveLogic>().isGrabMoving)
        {
            StartVisualization();
        }
        else
        {
            ClearVisualization();
        }
    }

    private void SetUpVisualization()
    {
        // Set up line renderer
        GameObject visualization = new GameObject("LineVisualizer");
        visualization.transform.SetParent(transform);
        
        m_LineRenderer = visualization.AddComponent<LineRenderer>();
        m_LineRenderer.positionCount = 2;
        m_LineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        m_LineRenderer.startColor = m_LineColor;
        m_LineRenderer.endColor = m_LineColor;
        m_LineRenderer.startWidth = 0.001f;
        m_LineRenderer.endWidth = 0.001f;
        m_LineRenderer.useWorldSpace = true;
        
        // Set up text mesh
        // m_ScaleText = visualization.AddComponent<TextMeshPro>();
        // m_ScaleText.text = ".";
        // m_ScaleText.color = m_PivotColor;
        // m_ScaleText.alignment = TextAlignmentOptions.Center;
    }

    private void StartVisualization()
    {
        m_LineRenderer.enabled = true;
        m_LineRenderer.SetPosition(0, m_LeftTransform.position);
        m_LineRenderer.SetPosition(1, m_RightTransform.position);
        
        Vector3 pivot = (m_RightTransform.position - m_LeftTransform.position) / 2;
        // m_ScaleText.enabled = true;
        // m_ScaleText.transform.position = pivot;
    }

    private void ClearVisualization()
    {
        m_LineRenderer.enabled = false;
        // m_ScaleText.enabled = false;
    }

    private void StartVibrationFeedback()
    {
        
    }
}
