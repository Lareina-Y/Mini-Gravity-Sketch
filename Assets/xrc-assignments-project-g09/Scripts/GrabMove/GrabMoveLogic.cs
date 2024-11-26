using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the main logic for the Grab Move locomotion technique.
/// Holds a reference to controllers and calculates the XR Origin's transform.
/// </summary>
public class GrabMoveLogic : MonoBehaviour
{
    [SerializeField] private List<Transform> m_Environment;

    [SerializeField] private Transform m_LeftTransform;
    [SerializeField] private Transform m_RightTransform;
    
    [SerializeField] private Transform m_XROrigin;
    
    [SerializeField] private float m_MinimumScale = 0.01f;
    [SerializeField] private float m_MaximumScale = 10f;
    
    [SerializeField] private bool m_UpdateOffset = true;
    
    private Vector3 m_PreviousLeftPosition;
    private Vector3 m_PreviousRightPosition;
    private Vector3 m_PreviousLeftForward;
    private Vector3 m_PreviousRightForward;

    private bool m_IsGrabMoving = false;
    private float m_CurrScale = 1f;
    
    public Transform leftTransform => m_LeftTransform;
    public Transform rightTransform => m_RightTransform;
    public bool isGrabMoving => m_IsGrabMoving;
    public float currentScale => m_CurrScale;

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
        m_PreviousLeftForward = m_LeftTransform.forward;
        m_PreviousRightForward = m_RightTransform.forward;
    }

    public void StartGrabMove()
    {
        m_IsGrabMoving = true;
    }

    public void EndGrabMove()
    {
        m_IsGrabMoving = false;
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
        Vector3 currVector = m_RightTransform.position - m_LeftTransform.position;
        Vector3 prevVector = m_PreviousRightPosition - m_PreviousLeftPosition;
        
        Vector3 currPivot = (m_LeftTransform.position + m_RightTransform.position) / 2;
        Vector3 axis = Vector3.Cross(prevVector, currVector).normalized;
        m_XROrigin.RotateAround(currPivot, - axis, Vector3.Angle(prevVector, currVector));
        
        // 2nd type of rotation
        Vector3 currForward = (m_RightTransform.forward + m_LeftTransform.forward) / 2;
        Vector3 prevForward = (m_PreviousRightForward + m_PreviousLeftForward) / 2;
        
        Vector3 planeNormal = (m_RightTransform.position - m_LeftTransform.position).normalized;
        Vector3 currProjectedForward = Vector3.ProjectOnPlane(currForward, planeNormal);
        Vector3 prevProjectedForward = Vector3.ProjectOnPlane(prevForward, planeNormal);
        
        float signedAngle = Vector3.SignedAngle(prevProjectedForward, currProjectedForward, planeNormal);
        
        m_XROrigin.RotateAround(currPivot, planeNormal, - signedAngle);
    }

    private void Scale()
    {
        float currDistance = Vector3.Distance(m_LeftTransform.position, m_RightTransform.position);
        float prevDistance = Vector3.Distance(m_PreviousLeftPosition, m_PreviousRightPosition);
        float prevScale = m_XROrigin.localScale.x;
        m_CurrScale = prevScale * prevDistance / currDistance;
        m_CurrScale = Mathf.Clamp(m_CurrScale, m_MinimumScale, m_MaximumScale);
        m_XROrigin.localScale = Vector3.one * m_CurrScale;
        
        // Add offset
        if (m_UpdateOffset)
        {
            Vector3 currPivot = (m_LeftTransform.position + m_RightTransform.position) / 2;
            Vector3 prevPivot = (m_PreviousLeftPosition + m_PreviousRightPosition) / 2;
            m_XROrigin.position += prevPivot - currPivot;
        }
    }
}
