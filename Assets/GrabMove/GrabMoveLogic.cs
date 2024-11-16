using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrabMoveLogic : MonoBehaviour
{
    [SerializeField] private List<Transform> m_Environment;

    [SerializeField] private Transform m_LeftTransform;
    [SerializeField] private Transform m_RightTransform;
    
    [SerializeField] private Transform m_XROrigin;
    
    [SerializeField] private float m_MinimumScale = 0.01f;
    [SerializeField] private float m_MaximumScale = 10f;
    
    [SerializeField] private bool m_UpdateOffset;
    
    public Transform leftTransform => m_LeftTransform;
    public Transform rightTransform => m_RightTransform;
    
    private Vector3 m_PreviousLeftPosition;
    private Vector3 m_PreviousRightPosition;
    // private Vector3 m_PreviousPivot;

    private bool m_IsGrabMoving = false;
    public bool isGrabMoving
    {
        get => m_IsGrabMoving;
        set => m_IsGrabMoving = value;
    }

    void Start()
    {
        foreach (Transform t in m_Environment)
        {
            t.SetParent(m_XROrigin);
        }
    }

    void Update()
    {
        if (m_IsGrabMoving)
        {
            Translation();
            Rotation();
            Scale();
        }
        
        m_PreviousLeftPosition = m_LeftTransform.position;
        m_PreviousRightPosition = m_RightTransform.position;
        // m_PreviousPivot = (m_PreviousLeftPosition + m_PreviousRightPosition) / 2;
    }

    private void Translation()
    {
        Vector3 currPivot = (m_LeftTransform.position + m_RightTransform.position) / 2;
        Vector3 prevPivot = (m_PreviousLeftPosition + m_PreviousRightPosition) / 2;
        m_XROrigin.position += prevPivot - currPivot;
    }

    private void Rotation()
    {
        // 1st type of rotation
        // TODO: check axis
        Vector3 currVector = m_RightTransform.position - m_LeftTransform.position;
        Vector3 prevVector = m_PreviousRightPosition - m_PreviousLeftPosition;
        Quaternion rotation = Quaternion.FromToRotation(currVector, prevVector);
        Vector3 currPivot = (m_LeftTransform.position + m_RightTransform.position) / 2;
        m_XROrigin.RotateAround(currPivot, Vector3.up, rotation.eulerAngles.y);
        
        // TODO: 2nd type of rotation
        // Vector3 rotationAxis = (m_RightTransform.position - m_LeftTransform.position).normalized;
        // float angle = Vector3.SignedAngle(m_LeftTransform.forward, m_RightTransform.forward, rotationAxis);
        // m_XROrigin.RotateAround(m_PreviousPivot, rotationAxis, angle);
    }

    private void Scale()
    {
        float currDistance = Vector3.Distance(m_LeftTransform.position, m_RightTransform.position);
        float prevDistance = Vector3.Distance(m_PreviousLeftPosition, m_PreviousRightPosition);
        float prevScale = m_XROrigin.localScale.x;
        float currScale = prevScale * prevDistance / currDistance;
        currScale = Mathf.Clamp(currScale, m_MinimumScale, m_MaximumScale);
        m_XROrigin.localScale = Vector3.one * currScale;
        
        // Add offset
        Vector3 currPivot = (m_LeftTransform.position + m_RightTransform.position) / 2;
        Vector3 prevPivot = (m_PreviousLeftPosition + m_PreviousRightPosition) / 2;
        m_XROrigin.position += prevPivot - currPivot;
    }
}
