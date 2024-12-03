using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class AxisScaleController : MonoBehaviour
{
    private Transform xAxis;
    private Transform yAxis;
    private Transform zAxis;

    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference rightGripAction;

    // Scale Mode State
    private static bool isInScaleMode = false;
    public static bool IsInScaleMode
    {
        get { return isInScaleMode; }
        set
        {
            isInScaleMode = value;
            Debug.Log($"Scale Mode changed to: {value}");
        }
    }

    // Current Scaling State
    private Transform currentSelectedAxis;
    private Vector3 initialControllerPosition;
    private Vector3 initialScale;
    private bool isScaling = false;

    private void Start()
    {
        Debug.Log("AxisScaleController Starting...");
        InitializeInputActions();
        InitializeAxes();
    }

    private void OnEnable()
    {
        if (rightGripAction != null)
        {
            rightGripAction.action.Enable();
            Debug.Log("Right grip action enabled in OnEnable.");
        }
    }

    private void OnDisable()
    {
        if (rightGripAction != null)
        {
            rightGripAction.action.Disable();
            Debug.Log("Right grip action disabled in OnDisable.");
        }
    }

    private void InitializeInputActions()
    {
        if (rightGripAction == null)
        {
            Debug.LogError("Right Grip Action is not assigned in the Inspector!");
            return;
        }

        rightGripAction.action.Enable();
        Debug.Log("Grip action enabled in InitializeInputActions.");
    }

    private void InitializeAxes()
    {
        Debug.Log("Initializing axes...");
        Transform axisParent = transform.Find("Axis");
        if (axisParent == null)
        {
            Debug.LogError("Axis parent not found under this object!");
            return;
        }

        xAxis = axisParent.Find("X-Axis");
        yAxis = axisParent.Find("Y-Axis");
        zAxis = axisParent.Find("Z-Axis");

        if (xAxis == null || yAxis == null || zAxis == null)
        {
            Debug.LogError("One or more axes (X, Y, Z) are missing under the Axis parent.");
            return;
        }

        SetupAxisCollider(xAxis);
        SetupAxisCollider(yAxis);
        SetupAxisCollider(zAxis);
    }

    private void SetupAxisCollider(Transform axis)
    {
        if (axis == null)
        {
            Debug.LogError("SetupAxisCollider called with null axis.");
            return;
        }

        AxisCollisionHandler collisionHandler = axis.gameObject.GetComponent<AxisCollisionHandler>();
        if (collisionHandler == null)
        {
            collisionHandler = axis.gameObject.AddComponent<AxisCollisionHandler>();
            collisionHandler.Initialize(this, axis.name);
        }
    }

    public void HandleAxisCollision(string axisName, Collider other, bool isEntering)
    {
        Debug.Log($"HandleAxisCollision: HandleAxisCollision called for {axisName} with isEntering = {isEntering}");

        if (!isInScaleMode)
        {
            Debug.Log("HandleAxisCollision: Not in scale mode, ignoring collision.");
            return;
        }

        if (!other.gameObject.name.Contains("Direct Interactor"))
        {
            Debug.Log($"HandleAxisCollision: Collider is not an interactor: {other.gameObject.name}");
            return;
        }

        if (isEntering)
        {
            Debug.Log($"HandleAxisCollision: Handling collision enter for {axisName}");

            if (rightGripAction == null || rightGripAction.action == null)
            {
                Debug.LogError("HandleAxisCollision: RightGripAction or its action is null in HandleAxisCollision.");
                return;
            }

            if (!rightGripAction.action.IsPressed())
            {
                Debug.Log("HandleAxisCollision: Grip conditions not met for axis selection.");
                return;
            }

            switch (axisName)
            {
                case "X-Axis":
                    currentSelectedAxis = xAxis;
                    break;
                case "Y-Axis":
                    currentSelectedAxis = yAxis;
                    break;
                case "Z-Axis":
                    currentSelectedAxis = zAxis;
                    break;
                default:
                    Debug.LogError($"HandleAxisCollision: Unknown axis: {axisName}");
                    return;
            }

            if (currentSelectedAxis != null)
            {
                isScaling = true;
                initialControllerPosition = other.transform.position;
                initialScale = transform.localScale;
                Debug.Log($"HandleAxisCollision: Scaling setup complete. Axis: {axisName}, Initial Position: {initialControllerPosition}, Initial Scale: {initialScale}");
            }
        }
        else
        {
            Debug.Log($"HandleAxisCollision: Collision exit for {axisName}");
            StopScaling();
        }
    }

    private void Update()
    {
        if (!isScaling)
        {
            return;
        }

        if (rightGripAction == null || !rightGripAction.action.IsPressed())
        {
            StopScaling();
            return;
        }

        // Scaling logic
        Vector3 currentControllerPosition = currentSelectedAxis.position;
        Debug.Log($"Current controller position: {currentControllerPosition}");

        float delta = 0;
        if (currentSelectedAxis == xAxis)
        {
            delta = currentControllerPosition.x - initialControllerPosition.x;
        }
        else if (currentSelectedAxis == yAxis)
        {
            delta = currentControllerPosition.y - initialControllerPosition.y;
        }
        else if (currentSelectedAxis == zAxis)
        {
            delta = currentControllerPosition.z - initialControllerPosition.z;
        }
        Debug.Log($"Delta calculated for {currentSelectedAxis.name}: {delta}");

        Vector3 newScale = initialScale;
        if (currentSelectedAxis == xAxis)
        {
            newScale.x = Mathf.Clamp(initialScale.x + delta, 0.1f, 5.0f);
        }
        else if (currentSelectedAxis == yAxis)
        {
            newScale.y = Mathf.Clamp(initialScale.y + delta, 0.1f, 5.0f);
        }
        else if (currentSelectedAxis == zAxis)
        {
            newScale.z = Mathf.Clamp(initialScale.z + delta, 0.1f, 5.0f);
        }

        transform.localScale = newScale;
        Debug.Log($"Scaling applied. New Scale: {newScale}");
    }

    private void StopScaling()
    {
        isScaling = false;
        currentSelectedAxis = null;
        Debug.Log("Stopped scaling.");
    }
}
