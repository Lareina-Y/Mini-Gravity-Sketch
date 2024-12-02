using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class TriggerState : MonoBehaviour
{
    [SerializeField]
    private InputActionProperty m_LeftTriggerAction;

    [SerializeField]
    private InputActionProperty m_RightTriggerAction;

    [SerializeField]
    private Renderer leftTriggerRenderer; // Renderer of the left trigger
    
    [SerializeField]
    private Renderer rightTriggerRenderer; // Renderer of the left trigger


    [SerializeField]
    private Color pressedColor = Color.green; // Color when trigger is pressed

    [SerializeField]
    private Color defaultColor = Color.grey; // Default color

    // Enable the input action
    protected void OnEnable()
    {
        m_LeftTriggerAction.action.Enable();
        m_RightTriggerAction.action.Enable();
    }

    // Disable the input action
    protected void OnDisable()
    {
        m_LeftTriggerAction.action.Disable();
        m_RightTriggerAction.action.Disable();
    }

    // Initialize the default color and set up the input event
    void Start()
    {
        // Set the default color on start
        if (leftTriggerRenderer != null)
        {
            leftTriggerRenderer.material.color = defaultColor;
        }
        
        if (rightTriggerRenderer != null)
        {
            rightTriggerRenderer.material.color = defaultColor;
        }

        // Listen for grip action input
        m_LeftTriggerAction.action.performed += OnLeftTriggerPerformed;
        m_LeftTriggerAction.action.canceled += OnLeftTriggerCanceled;
        m_RightTriggerAction.action.performed += OnRightTriggerPerformed;
        m_RightTriggerAction.action.canceled += OnRightTriggerCanceled;
    }

    // Change color when the left trigger is performed
    private void OnLeftTriggerPerformed(InputAction.CallbackContext context)
    {
        leftTriggerRenderer.material.color = pressedColor;
    }

    // Revert color when the left trigger is released
    private void OnLeftTriggerCanceled(InputAction.CallbackContext context)
    {
        leftTriggerRenderer.material.color = defaultColor;
    }
    
    // Change color when the right trigger is performed
    private void OnRightTriggerPerformed(InputAction.CallbackContext context)
    {
        rightTriggerRenderer.material.color = pressedColor;
    }

    // Revert color when the right trigger is released
    private void OnRightTriggerCanceled(InputAction.CallbackContext context)
    {
        rightTriggerRenderer.material.color = defaultColor;
    }

}