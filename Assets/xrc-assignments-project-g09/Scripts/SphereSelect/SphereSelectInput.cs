using UnityEngine;
using UnityEngine.InputSystem;

public class SphereSelectInput : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty changeRadius;
    [SerializeField] private float radiusChangeSpeed = 0.1f;
    private SphereSelectLogic sphereSelectLogic;
    private bool isAtDefaultRadius = false;

    private void Start()
    {
        sphereSelectLogic = GetComponent<SphereSelectLogic>();
    }

    private void OnEnable()
    {
        changeRadius.action.Enable();
        changeRadius.action.performed += OnRadiusChange;
        changeRadius.action.canceled += OnRadiusChangeReleased;
    }

    private void OnDisable()
    {
        changeRadius.action.Disable();
        changeRadius.action.performed -= OnRadiusChange;
        changeRadius.action.canceled -= OnRadiusChangeReleased;
    }

    private void OnRadiusChange(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        float inputY = input.y;

        // If the radius is at the default value, don't change it
        if (isAtDefaultRadius && Mathf.Approximately(sphereSelectLogic.CurrentRadius, sphereSelectLogic.DefaultRadius))
        {
            return;
        }

        float currentRadius = sphereSelectLogic.CurrentRadius;
        float deltaRadius = inputY * radiusChangeSpeed * Time.deltaTime;
        float newRadius = currentRadius + deltaRadius;

        // If the radius is at the default value, set it to the default value
        if ((newRadius >= sphereSelectLogic.DefaultRadius && currentRadius < sphereSelectLogic.DefaultRadius) ||
            (newRadius <= sphereSelectLogic.DefaultRadius && currentRadius > sphereSelectLogic.DefaultRadius))
        {
            newRadius = sphereSelectLogic.DefaultRadius;
            isAtDefaultRadius = true;
        }
        else
        {
            isAtDefaultRadius = false;
        }

        sphereSelectLogic.AdjustRadius(newRadius - currentRadius);
    }

    private void OnRadiusChangeReleased(InputAction.CallbackContext context)
    {
        isAtDefaultRadius = false; // Allow the sphere to continue scaling
    }
}
