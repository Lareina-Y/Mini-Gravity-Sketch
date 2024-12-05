using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ButtonsState : MonoBehaviour
{
    
    [SerializeField]
    private XRBaseInteractor m_Interactor;
    
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
    private InputActionProperty m_RightTriggerAction;
    
    [SerializeField]
    private InputActionProperty m_RightGripAction;
    
    [SerializeField]
    private Transform leftButtonATransform;

    [SerializeField]
    private Transform leftButtonBTransform;

    [SerializeField]
    private Transform rightButtonATransform;

    [SerializeField]
    private Transform rightButtonBTransform;

    [SerializeField]
    private Transform leftHomeButtonTransform;

    [SerializeField]
    private Transform rightHomeButtonTransform;

    [SerializeField]
    private Transform leftThumbstickTransform;
    
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
    private Color defaultColor = Color.grey;

    private Transform[] allSpritesTransforms;
    
    protected void OnEnable()
    {
        m_LeftButtonAAction.action.Enable();
        m_RightButtonAAction.action.Enable();
        m_LeftButtonBAction.action.Enable();
        m_RightButtonBAction.action.Enable();
        m_leftHomeButtonAction.action.Enable();
        m_rightHomeButtonAction.action.Enable();
        m_RightTriggerAction.action.Enable();
        m_RightGripAction.action.Enable();
    }

    protected void OnDisable()
    {
        m_LeftButtonAAction.action.Disable();
        m_RightButtonAAction.action.Disable();
        m_LeftButtonBAction.action.Disable();
        m_RightButtonBAction.action.Disable();
        m_leftHomeButtonAction.action.Disable();
        m_rightHomeButtonAction.action.Disable();
        m_RightTriggerAction.action.Disable();
        m_RightGripAction.action.Disable();
    }

    private void Start()
    {

        allSpritesTransforms = new []
        {
            leftButtonATransform,
            rightButtonATransform,
            leftButtonBTransform,
            rightButtonBTransform,
            rightHomeButtonTransform,
            leftThumbstickTransform,
        };

        // Initialize all button colors
        SetButtonColor(leftButtonATransform, defaultColor);
        SetButtonColor(rightButtonATransform, defaultColor);
        SetButtonColor(leftButtonBTransform, defaultColor);
        SetButtonColor(rightButtonBTransform, defaultColor);
        SetButtonColor(leftHomeButtonTransform, defaultColor);
        SetButtonColor(rightHomeButtonTransform, defaultColor);

        // Attach input event listeners
        m_LeftButtonAAction.action.performed += context => OnButtonPerformed(leftButtonATransform, drawFeedbackColor);
        m_LeftButtonAAction.action.canceled += context => OnButtonCanceled(leftButtonATransform);

        m_RightButtonAAction.action.performed +=
            context => OnButtonPerformed(rightButtonATransform, colorPlateFeedbackColor);
        m_RightButtonAAction.action.canceled += context => OnButtonCanceled(rightButtonATransform);

        m_LeftButtonBAction.action.performed += context => OnButtonPerformed(leftButtonBTransform, menuFeedbackColor);
        m_LeftButtonBAction.action.canceled += context => OnButtonCanceled(leftButtonBTransform);

        m_RightButtonBAction.action.performed +=
            context => OnButtonPerformed(rightButtonBTransform, deleteFeedbackColor);
        m_RightButtonBAction.action.canceled += context => OnButtonCanceled(rightButtonBTransform);

        m_leftHomeButtonAction.action.performed +=
            context => OnButtonPerformed(leftHomeButtonTransform, homeFeedbackColor);
        m_leftHomeButtonAction.action.canceled += context => OnButtonCanceled(leftHomeButtonTransform);

        m_rightHomeButtonAction.action.performed +=
            context => OnButtonPerformed(rightHomeButtonTransform, homeFeedbackColor);
        m_rightHomeButtonAction.action.canceled += context => OnButtonCanceled(rightHomeButtonTransform);

        m_RightTriggerAction.action.performed += context => HideSpecificSprites(allSpritesTransforms);
        m_RightTriggerAction.action.canceled += context => ShowSpecificSprites(allSpritesTransforms);

        m_RightGripAction.action.performed += context => rightGripActionPerformed();
        m_RightGripAction.action.canceled += context => ShowSpecificSprites(allSpritesTransforms);
    }

    private void OnButtonPerformed(Transform buttonTransform, Color feedbackColor)
    {
        SetButtonColor(buttonTransform, feedbackColor);
    }

    private void OnButtonCanceled(Transform buttonTransform)
    {
        SetButtonColor(buttonTransform, defaultColor);
    }

    private void rightGripActionPerformed()
    {
        if (m_Interactor.hasSelection)
        {
            HideSpecificSprites(leftButtonATransform, leftButtonBTransform, leftThumbstickTransform);
        }
    }
    
    private void ShowSpecificSprites(params Transform[] buttonTransforms)
    {
        foreach (var buttonTransform in buttonTransforms)
        {
            if (buttonTransform != null)
            {
                ToggleChildSprites(buttonTransform, true);
            }
        }
    }

    private void HideSpecificSprites(params Transform[] buttonTransforms)
    {
        foreach (var buttonTransform in buttonTransforms)
        {
            if (buttonTransform != null)
            {
                ToggleChildSprites(buttonTransform, false);
            }
        }
    }
    
    private void ToggleChildSprites(Transform parentTransform, bool state)
    {
        foreach (Transform child in parentTransform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = state;
            }
        }
    }

    private void SetButtonColor(Transform buttonTransform, Color color)
    {
        Renderer renderer = buttonTransform.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}
