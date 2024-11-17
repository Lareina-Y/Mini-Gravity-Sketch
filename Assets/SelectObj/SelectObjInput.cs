using UnityEngine;
using UnityEngine.InputSystem;

public class SelectObjInput : MonoBehaviour
{
    public float inputSensitivity = 0.01f;
    public InputActionProperty changeRadius;

    private SelectObjLogic _selectObjLogic;

    private void Awake()
    {
        _selectObjLogic = GetComponent<SelectObjLogic>();
        changeRadius.action.performed += OnRadiusChange;
    }

    private void OnEnable()
    {
        changeRadius.action.Enable();
    }

    private void OnDisable()
    {
        changeRadius.action.Disable();
    }

    private void OnRadiusChange(InputAction.CallbackContext context)
    {
        // Read the input value and adjust the sphere's radius
        Vector2 axisValue = context.ReadValue<Vector2>();
        float radiusChange = axisValue.y;

        if (_selectObjLogic != null)
        {
            _selectObjLogic.AdjustRadius(radiusChange * inputSensitivity);
        }
    }
}
