using UnityEngine;
using UnityEngine.InputSystem;

public class SphereSelectInput : MonoBehaviour
{
    [SerializeField] private float inputSensitivity = 1f;
    [SerializeField] private InputActionProperty changeRadius;

    private SphereSelectLogic _sphereSelectLogic;

    private void Awake()
    {
        _sphereSelectLogic = GetComponent<SphereSelectLogic>();
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

        if (_sphereSelectLogic != null)
        {
            _sphereSelectLogic.AdjustRadius(radiusChange * inputSensitivity * Time.deltaTime);
        }
    }
}
