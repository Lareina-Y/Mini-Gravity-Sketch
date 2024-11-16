using UnityEngine;

public class GrabMoveFeedback : MonoBehaviour
{
    [SerializeField] Color m_PivotColor;
    [SerializeField] Color m_LineColor;
    
    private Transform m_LeftTransform;
    private Transform m_RightTransform;

    void Awake()
    {
        m_LeftTransform = GetComponent<GrabMoveLogic>().leftTransform;
        m_RightTransform = GetComponent<GrabMoveLogic>().rightTransform;
    }
}
