using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ButtonsState : MonoBehaviour
{
    [SerializeField]
    private InputActionProperty m_LeftButtonAAction;
    
    [SerializeField]
    private InputActionProperty m_LeftButtonBAction;
    
    [SerializeField]
    private InputActionProperty m_RightButtonAAction;
    
    [SerializeField]
    private InputActionProperty m_RightButtonBAction;
    
    [SerializeField]
    private InputActionProperty m_leftHomeButtonAction;
    
    [SerializeField]
    private InputActionProperty m_rightHomeButtonAction;
    
    [SerializeField]
    private Renderer leftButtonARenderer;
    
    [SerializeField]
    private Renderer leftButtonBRenderer;
    
    [SerializeField]
    private Renderer rightButtonARenderer;
    
    [SerializeField]
    private Renderer rightButtonBRenderer;
    
    [SerializeField]
    private Renderer leftHomeButtonRenderer;
    
    [SerializeField]
    private Renderer rightHomeButtonRenderer;
    
    [SerializeField]
    private Color deleteFeedbackColor = Color.red;
    
    [SerializeField]
    private Color menuFeedbackColor = Color.blue;
    
    [SerializeField]
    private Color drawFeedbackColor = Color.magenta;
    
    [SerializeField]
    private Color colorPlateFeedbackColor = Color.cyan;
    
    [SerializeField]
    private Color homeFeedbackColor = Color.black;
    
    [SerializeField]
    private Color defaultColor = Color.grey; // Default color

    // Enable the input action
    protected void OnEnable()
    {
        m_LeftButtonAAction.action.Enable();
        m_RightButtonAAction.action.Enable();
        m_LeftButtonBAction.action.Enable();
        m_RightButtonBAction.action.Enable();
        m_leftHomeButtonAction.action.Enable();
        m_rightHomeButtonAction.action.Enable();
    }

    // Disable the input action
    protected void OnDisable()
    {
        m_LeftButtonAAction.action.Disable();
        m_RightButtonAAction.action.Disable();
        m_LeftButtonBAction.action.Disable();
        m_RightButtonBAction.action.Disable();
        m_leftHomeButtonAction.action.Disable();
        m_rightHomeButtonAction.action.Disable();
    }
    
    private void Start()
    {
        // Set the default color on start
        SetRendererColor(leftButtonARenderer, defaultColor);
        SetRendererColor(rightButtonARenderer, defaultColor);
        SetRendererColor(leftButtonBRenderer, defaultColor);
        SetRendererColor(rightButtonBRenderer, defaultColor);
        SetRendererColor(leftHomeButtonRenderer, defaultColor);
        SetRendererColor(rightHomeButtonRenderer, defaultColor);

        // Listen for button input events
        m_LeftButtonAAction.action.performed += context => OnButtonPerformed(leftButtonARenderer, drawFeedbackColor);
        m_LeftButtonAAction.action.canceled += context => OnButtonCanceled(leftButtonARenderer);

        m_RightButtonAAction.action.performed += context => OnButtonPerformed(rightButtonARenderer, colorPlateFeedbackColor);
        m_RightButtonAAction.action.canceled += context => OnButtonCanceled(rightButtonARenderer);

        m_LeftButtonBAction.action.performed += context => OnButtonPerformed(leftButtonBRenderer, menuFeedbackColor);
        m_LeftButtonBAction.action.canceled += context => OnButtonCanceled(leftButtonBRenderer);

        m_RightButtonBAction.action.performed += context => OnButtonPerformed(rightButtonBRenderer, deleteFeedbackColor);
        m_RightButtonBAction.action.canceled += context => OnButtonCanceled(rightButtonBRenderer);
        
        m_leftHomeButtonAction.action.performed += context => OnButtonPerformed(leftHomeButtonRenderer, homeFeedbackColor);
        m_leftHomeButtonAction.action.canceled += context => OnButtonCanceled(leftHomeButtonRenderer);
        
        m_rightHomeButtonAction.action.performed += context => OnButtonPerformed(rightHomeButtonRenderer, homeFeedbackColor);
        m_rightHomeButtonAction.action.canceled += context => OnButtonCanceled(rightHomeButtonRenderer);
        
    }

    private void OnButtonPerformed(Renderer buttonRenderer, Color feedbackColor)
    {
        SetRendererColor(buttonRenderer, feedbackColor);
    }

    private void OnButtonCanceled(Renderer buttonRenderer)
    {
        SetRendererColor(buttonRenderer, defaultColor);
    }

    private void SetRendererColor(Renderer buttonRenderer, Color color)
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = color;
        }
    }

}