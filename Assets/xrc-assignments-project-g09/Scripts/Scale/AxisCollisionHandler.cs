using UnityEngine;
using UnityEngine.InputSystem;


public class AxisCollisionHandler : MonoBehaviour
{
    private AxisScaleController controller;
    private string axisName;
    [Header("Input Actions")]
    [SerializeField]


    private InputActionReference rightGripAction;

    public void Initialize(AxisScaleController controller, string axisName)
    {
        this.controller = controller;
        this.axisName = axisName;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (AxisScaleController.IsInScaleMode && rightGripAction.action.IsPressed())
        {
            Debug.Log($"{axisName} detected collision with: {other.gameObject.name}");
            controller.HandleAxisCollision(axisName, other, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (AxisScaleController.IsInScaleMode)
        {
            Debug.Log($"{axisName} ended collision with: {other.gameObject.name}");
            controller.HandleAxisCollision(axisName, other, false);
        }
    }


}
