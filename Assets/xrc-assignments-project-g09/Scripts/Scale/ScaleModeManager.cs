using UnityEngine;
using UnityEngine.InputSystem;

public class ScaleModeManager : MonoBehaviour
{
    private bool isScaleMode = false;
    private bool isGripPressed = false;  

    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference leftYButtonAction;
    [SerializeField]
    private InputActionReference rightGripAction;

    void Start()
    {
        Debug.Log("ScaleModeManager Starting...");

        if (leftYButtonAction != null && rightGripAction != null)
        {
            leftYButtonAction.action.Enable();
            rightGripAction.action.Enable();

       
            leftYButtonAction.action.performed += OnScaleToggle;
            rightGripAction.action.performed += OnGripPressed;    
            rightGripAction.action.canceled += OnGripReleased;  

            Debug.Log("Input actions setup complete");
        }
        else
        {
            Debug.LogError("Input actions not fully assigned!");
        }
        Debug.Log("ScaleModeManager initialization complete");
    }

    private void OnGripPressed(InputAction.CallbackContext context)
    {
        isGripPressed = true;
        Debug.Log("Grip Pressed!");
    }

    private void OnGripReleased(InputAction.CallbackContext context)
    {
        isGripPressed = false;
        Debug.Log("Grip Released!");
    }

    private void OnScaleToggle(InputAction.CallbackContext context)
    {
        Debug.Log("Left Y pressed!");
        Debug.Log($"Current Scale Mode: {isScaleMode}, Grip State: {isGripPressed}");

        if (!isScaleMode)
        {
            if (isGripPressed)  
            {
                isScaleMode = true;
                AxisScaleController.IsInScaleMode = true;
                Debug.Log("Entered Scale Mode, Grab Mode ended");
            }
            else
            {
                Debug.Log("Cannot enter Scale Mode - Grip not pressed");
            }
        }
        else
        {
            isScaleMode = false;
            AxisScaleController.IsInScaleMode = false;
            Debug.Log("Exited Scale Mode");
        }
    }

    void OnDestroy()
    {
        if (leftYButtonAction != null)
        {
            leftYButtonAction.action.performed -= OnScaleToggle;
            leftYButtonAction.action.Disable();
        }
        if (rightGripAction != null)
        {
            rightGripAction.action.performed -= OnGripPressed;
            rightGripAction.action.canceled -= OnGripReleased;
            rightGripAction.action.Disable();
        }
    }
}