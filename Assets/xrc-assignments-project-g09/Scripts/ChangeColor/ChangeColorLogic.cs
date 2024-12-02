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
    
    private ColorPanel m_ColorPanel;
    private Color m_Color;
    private GameObject m_TargetObject;

    private bool m_IsChanging;
    
    public XRBaseInteractor interactor => m_Interactor;
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
        m_IsChanging = true;
    }

    public void EndChangeColor()
    {
        m_ColorPanel.SetActive(false);
        m_IsChanging = false;
    }
}
