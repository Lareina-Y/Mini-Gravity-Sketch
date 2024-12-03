using UnityEngine;
using UnityEngine.InputSystem;

public class AxisCollisionHandler : MonoBehaviour
{
    private AxisScaleController controller;
    private bool isTriggered;
    private string axisName;
    private Collider otherObject;
    
    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference rightGripAction;

    private void Start()
    {

        controller = GetComponentInParent<AxisScaleController>();
        axisName = gameObject.name;  
        
        if (controller == null)
        {
            Debug.LogError($"Cannot find AxisScaleController for {axisName}");
        }
        else
        {
            Debug.Log($"Successfully initialized {axisName} with controller");
        }


        if (rightGripAction != null)
        {
            rightGripAction.action.Enable();
            Debug.Log($"[{axisName}] Right grip action enabled");
        }
        else
        {
            Debug.LogError($"[{axisName}] Right grip action not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[{axisName}] Trigger Enter with {other.gameObject.name}");
        if (AxisScaleController.IsInScaleMode)
        {
            isTriggered = true;
            otherObject = other;
            Debug.Log($"[{axisName}] Triggered: {isTriggered}, Other: {otherObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[{axisName}] Trigger Exit");
        if (AxisScaleController.IsInScaleMode)
        {
            isTriggered = false;
            otherObject = null;
            Debug.Log($"[{axisName}] Reset trigger state");
        }
    }

    private void Update()
    {
        // Debug.Log($"[{axisName}] Status:" +
        //     $"\n - Controller: {controller != null}" +
        //     $"\n - Scale Mode: {AxisScaleController.IsInScaleMode}" +
        //     $"\n - Is Triggered: {isTriggered}" +
        //     $"\n - Grip Action: {(rightGripAction != null ? "Set" : "Not Set")}" +
        //     $"\n - Grip Pressed: {(rightGripAction != null ? rightGripAction.action.IsPressed().ToString() : "N/A")}");

        if (controller != null && AxisScaleController.IsInScaleMode && isTriggered)
        {
            Debug.Log($"[{axisName}] Base conditions met");
            if (rightGripAction != null && rightGripAction.action.IsPressed())
            {
                Debug.Log($"[{axisName}] Grip pressed, handling collision");
                controller.HandleAxisCollision(axisName, otherObject, true);
            }
        }
    }
}
