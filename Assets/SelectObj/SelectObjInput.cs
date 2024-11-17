using UnityEngine;
using UnityEngine.InputSystem;

public class SelectObjInput : MonoBehaviour
{
    public float inputSensitivity = 1f;
    public InputActionProperty changeRadius;

    private SelectObjLogic _selectObjLogic;

    private void Awake()
    {
        // Get reference to the SphereSelectLogic component
        _selectObjLogic = GetComponent<SelectObjLogic>();

        // Ensure the InputAction is enabled
        if (changeRadius != null)
        {
            changeRadius.action.Enable();
        }
    }

    private void OnEnable()
    {
        if (changeRadius != null)
        {
            // Subscribe to the input action's performed event
            changeRadius.action.performed += OnRadiusChange;
        }
    }

    private void OnDisable()
    {
        if (changeRadius != null)
        {
            // Unsubscribe from the input action's performed event
            changeRadius.action.performed -= OnRadiusChange;
        }
    }

    private void OnRadiusChange(InputAction.CallbackContext context)
    {
        // Read the input value and adjust the sphere's radius
        float inputValue = context.ReadValue<float>();

        if (_selectObjLogic != null)
        {
            _selectObjLogic.AdjustRadius(inputValue * inputSensitivity);
        }
    }
}
