using UnityEngine;
using UnityEngine.InputSystem;

public class AxisCollisionHandler : MonoBehaviour
{
    private AxisScaleController controller; // Dynamically assigned controller
    private bool isTriggered = false;       // Whether the axis is triggered
    private string axisName;               // Name of the axis (X, Y, Z)
    private Collider otherObject;          // The object that triggered the collider
    


    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference rightGripAction; // Input action for grip button

    /// <summary>
    /// Dynamically assign AxisScaleController to this handler.
    /// </summary>
    /// <param name="assignedController">The AxisScaleController managing scaling.</param>
    public void Initialize(AxisScaleController assignedController)
    {
        Debug.Log($"try to init the controller!!! {assignedController}");
        controller = assignedController;
        axisName = gameObject.name;

        if (controller == null)
        {
            Debug.LogError($"[{axisName}] AxisScaleController is not assigned!");
        }
        else
        {
            Debug.Log($"[{axisName}] Successfully assigned AxisScaleController.");
        }

        // Enable grip input action
        if (rightGripAction != null)
        {
            rightGripAction.action.Enable();
            Debug.Log($"[{axisName}] Right grip action enabled.");
        }
        else
        {
            Debug.LogError($"[{axisName}] Right grip action not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (controller != null && controller.IsInScaleMode)
        {
            isTriggered = true;
            otherObject = other;
            Debug.Log($"[{axisName}] Trigger Enter with {other.gameObject.name}, isTriggered: {isTriggered}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (controller != null && controller.IsInScaleMode)
        {
            isTriggered = false;
            otherObject = null;
            Debug.Log($"[{axisName}] Trigger Exit, reset trigger state");
        }
    }

    private void Update()
    {
        // Check if scaling conditions are met
        if (controller != null && controller.IsInScaleMode && isTriggered)
        {
            if (rightGripAction != null && rightGripAction.action.IsPressed())
            {
                Debug.Log($"[{axisName}] Grip pressed, handling collision");
                controller.HandleAxisCollision(axisName, otherObject, true);
            }
        }
    }

    private void OnDestroy()
    {
        // Disable grip input action to avoid memory leaks
        if (rightGripAction != null)
        {
            rightGripAction.action.Disable();
        }
    }
}
