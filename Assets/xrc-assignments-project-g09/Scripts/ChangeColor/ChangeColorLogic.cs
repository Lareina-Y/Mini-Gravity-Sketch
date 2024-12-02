using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Contains the main logic for Color Change.
/// </summary>
public class ChangeColorLogic : MonoBehaviour
{
    [SerializeField] private XRBaseInteractor m_Interactor;
    [SerializeField] private GameObject m_PointerPrefab;
    
    private ColorPanel m_ColorPanel;
    private Color m_Color;
    private GameObject m_Pointer;
    private GameObject m_TargetObject; // TODO: material issue

    private bool m_IsChanging;
    
    public XRBaseInteractor interactor => m_Interactor;
    
    // TODO: All future objects should be in this color
    public Color color => m_Color;

    void Awake()
    {
        m_ColorPanel = GetComponent<ColorPanel>();
    }

    void OnEnable()
    {
        m_Interactor.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        m_Interactor.selectEntered.RemoveListener(OnSelectEntered);
    }

    void Start()
    {
        // Spawn a pointer object
        m_Pointer = Instantiate(m_PointerPrefab, m_Interactor.transform);
        m_Pointer.transform.localPosition = new Vector3(0, 0, -0.006f);
        m_Pointer.SetActive(false);
    }

    void Update()
    {
        if (m_IsChanging)
        {
            m_Color = m_ColorPanel.color;
            m_TargetObject.GetComponent<Renderer>().material.color = m_Color;
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs eventArgs)
    {
        if (eventArgs.interactableObject is IXRSelectInteractable selectInteractable)
        {
            m_TargetObject = selectInteractable.transform.gameObject;
            
        }
    }

    public void ChangeColor()
    {
        m_ColorPanel.SetActive(true);
        m_Pointer.SetActive(true);
        m_IsChanging = true;
    }

    public void EndChangeColor()
    {
        m_ColorPanel.SetActive(false);
        m_Pointer.SetActive(false);
        m_IsChanging = false;
    }
}
