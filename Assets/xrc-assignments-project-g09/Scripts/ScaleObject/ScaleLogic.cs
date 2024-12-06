using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public enum ScaleType
{
    None,
    X,
    Y,
    Z
}

public class ScaleLogic : MonoBehaviour
{
    [SerializeField] private XRBaseInteractor m_Interactor;
    [SerializeField] private Transform m_RightTransform;
    private Vector3 m_PreviousRightPosition;
    
    [SerializeField] private GameObject m_XAxisPrefab;
    [SerializeField] private GameObject m_YAxisPrefab;
    [SerializeField] private GameObject m_ZAxisPrefab;

    public float m_X;
    public float m_Y;
    public float m_Z;
    
    private GameObject m_XAxis;
    private GameObject m_YAxis;
    private GameObject m_ZAxis;

    private GameObject m_TargetObject;
    public bool HasGameObject => m_TargetObject != null;
    
    private bool m_IsScaleActive = false;
    public bool IsScaleActive => m_IsScaleActive;

    private ScaleType m_ScaleType = ScaleType.None;
    
    void OnEnable()
    {
        m_Interactor.selectEntered.AddListener(OnSelectEntered);
        m_Interactor.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        m_Interactor.selectEntered.RemoveListener(OnSelectEntered);
        m_Interactor.selectExited.RemoveListener(OnSelectExited);
    }

    // Update the axis position every frame
    void Update()
    {
        if (m_IsScaleActive)
        {
            if (m_TargetObject != null)
            {
                UpdateScale();
            
                UpdateAxesRotation();
                UpdateAxesPosition();
            }
            else
            {
                EndScale();
            }
        }
        
        
        m_PreviousRightPosition = m_RightTransform.position;
    }

    private void OnSelectEntered(SelectEnterEventArgs eventArgs)
    {
        if (eventArgs.interactableObject is IXRSelectInteractable selectInteractable)
        {
            GameObject selected = selectInteractable.transform.gameObject;
            
            if (m_IsScaleActive == false)
            {
                m_TargetObject = selected;
                
                m_X = m_TargetObject.transform.localScale.x;
                m_Y = m_TargetObject.transform.localScale.y;
                m_Z = m_TargetObject.transform.localScale.z;
            }
            else
            {
                if (selected == m_XAxis)
                {
                    m_ScaleType = ScaleType.X;
                }
                else if (selected == m_YAxis)
                {
                    m_ScaleType = ScaleType.Y;
                }
                else if (selected == m_ZAxis)
                {
                    m_ScaleType = ScaleType.Z;
                }
            }
        }
    }
    

    private void OnSelectExited(SelectExitEventArgs eventArgs)
    {
        if (eventArgs.interactableObject is IXRSelectInteractable selectInteractable)
        { 
            m_ScaleType = ScaleType.None;
        }
    }

    public void StartScale()
    {
        if (m_TargetObject != null)
        {
            m_IsScaleActive = true;
            CreateAxes();
        }
    }

    public void EndScale()
    {
        m_IsScaleActive = false;
        m_TargetObject = null;
        DestroyAxes();
    }

    private void UpdateScale()
    {
        Vector3 transformDelta = m_RightTransform.position - m_PreviousRightPosition;
        float delta;
        if (m_ScaleType == ScaleType.X)
        {
           delta = Vector3.Dot(m_TargetObject.transform.right, transformDelta);
           m_X += delta;
        } 
        else if (m_ScaleType == ScaleType.Y)
        {
            delta = Vector3.Dot(m_TargetObject.transform.up, transformDelta);
            m_Y += delta;
        }
        else if (m_ScaleType == ScaleType.Z)
        {
            delta = Vector3.Dot(m_TargetObject.transform.forward, transformDelta);
            m_Z += delta;
        }
        
        m_TargetObject.transform.localScale = new Vector3(m_X, m_Y, m_Z);
    }

    private void CreateAxes()
    {
        m_XAxis = Instantiate(m_XAxisPrefab, transform);
        m_YAxis = Instantiate(m_YAxisPrefab, transform);
        m_ZAxis = Instantiate(m_ZAxisPrefab, transform);
        
        Vector3 scale = new Vector3(0.05f, 0.05f, 0.05f);
        m_XAxis.transform.localScale = scale;
        m_YAxis.transform.localScale = scale;
        m_ZAxis.transform.localScale = scale;
    }

    private void DestroyAxes()
    {
        if (m_XAxis != null)
        {
            Destroy(m_XAxis);
            m_XAxis = null;
        }

        if (m_YAxis != null)
        {
            Destroy(m_YAxis);
            m_YAxis = null;
        }

        if (m_ZAxis != null)
        {
            Destroy(m_ZAxis);
            m_ZAxis = null;
        }
    }
    
    private void UpdateAxesPosition()
    {
        Bounds objectBounds = m_TargetObject.GetComponent<MeshFilter>().mesh.bounds;
        
        Vector3 objectCenter = m_TargetObject.transform.TransformPoint(objectBounds.center);
        
        Vector3 xExtentWorld = m_TargetObject.transform.TransformVector(Vector3.right * objectBounds.extents.x) + m_TargetObject.transform.right * 0.05f;
        Vector3 yExtentWorld = m_TargetObject.transform.TransformVector(Vector3.up * objectBounds.extents.y) + m_TargetObject.transform.up * 0.05f;
        Vector3 zExtentWorld = m_TargetObject.transform.TransformVector(Vector3.forward * objectBounds.extents.z) + m_TargetObject.transform.forward * 0.05f;
        
        m_XAxis.transform.position = objectCenter + xExtentWorld;
        m_YAxis.transform.position = objectCenter + yExtentWorld;
        m_ZAxis.transform.position = objectCenter + zExtentWorld;
    }

    private void UpdateAxesRotation()
    {
        m_XAxis.transform.rotation = m_TargetObject.transform.rotation;
        m_YAxis.transform.rotation = m_TargetObject.transform.rotation;
        m_ZAxis.transform.rotation = m_TargetObject.transform.rotation;
        
        m_XAxis.transform.localRotation *= Quaternion.Euler(0, 0, 0);
        m_YAxis.transform.localRotation *= Quaternion.Euler(0, 0, 90);
        m_ZAxis.transform.localRotation *= Quaternion.Euler(0, -90, 0);
    }
}
