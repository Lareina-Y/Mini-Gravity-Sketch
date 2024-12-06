using UnityEngine;
using UnityEngine.InputSystem;

public class AxisScaleController : MonoBehaviour
{
    private Transform xAxis;
    private Transform yAxis;
    private Transform zAxis;
    private Transform targetObject; 

    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference rightGripAction;

    // Current Scaling State
    private Transform currentSelectedAxis;
    private Vector3 initialControllerPosition;
    private Vector3 initialScale;
    private bool isScaling = false;

    private Vector3 initialScaleX;
    private Vector3 initialScaleY;
    private Vector3 initialScaleZ;

    [Header("References")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;

    // Scale Mode State
    private bool isInScaleMode = false;
    public bool IsInScaleMode
    {
        get { return isInScaleMode; }
        set
        {
            isInScaleMode = value;
            Debug.Log($"[{gameObject.name}] Scale Mode changed to: {value}");
            UpdateAxisVisibility(value);
        }
    }

    public void SetAxes(Transform xAxisTransform, Transform yAxisTransform, Transform zAxisTransform)
    {
        xAxis = xAxisTransform;
        yAxis = yAxisTransform;
        zAxis = zAxisTransform;

        if (xAxis == null || yAxis == null || zAxis == null)
        {
            Debug.LogError("One or more axes are missing. Ensure all axis references are assigned.");
        }
        else
        {
            Debug.Log("Axes successfully assigned.");
        }

    }

    private void Start()
    {
        InitializeInputActions();
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

    public void UpdateAxisVisibility(bool visible)
    {
        if (xAxis != null) xAxis.gameObject.SetActive(visible);
        if (yAxis != null) yAxis.gameObject.SetActive(visible);
        if (zAxis != null) zAxis.gameObject.SetActive(visible);

        Debug.Log($"Axis visibility set to: {visible}");
    }

    public void HandleAxisCollision(string axisName, Collider other, bool isEntering)
{
    if (!isInScaleMode)
    {
        Debug.Log("Not in scale mode, ignoring collision.");
        return;
    }

    if (isEntering)
    {
        if (rightGripAction == null || rightGripAction.action == null)
        {
            Debug.LogError("RightGripAction is null.");
            return;
        }

        if (!rightGripAction.action.IsPressed())
        {
            Debug.Log("Grip not pressed.");
            return;
        }

        axisName = axisName.Replace("(Clone)", "").Trim();
        switch (axisName)
        {
            case "X-Axis":
                currentSelectedAxis = xAxis;
                initialScaleX = targetObject.localScale;
                break;
            case "Y-Axis":
                currentSelectedAxis = yAxis;
                initialScaleY = targetObject.localScale;
                break;
            case "Z-Axis":
                currentSelectedAxis = zAxis;
                initialScaleZ = targetObject.localScale;
                break;
            default:
                return;
        }

        isScaling = true;
        initialControllerPosition = other.transform.position;
        Debug.Log($"Started scaling {axisName}");
    }
    else
    {
        StopScaling();
    }
}


    private void StopScaling()
    {
        isScaling = false;
        currentSelectedAxis = null;
        Debug.Log("Stopped scaling");
    }

    private void Update()
    {
        if (isScaling && currentSelectedAxis != null && rightGripAction != null && rightGripAction.action.IsPressed() )
        {
            UpdateScaling();
        }
    }


private void UpdateScaling()
{
    if (currentSelectedAxis == null)
    {
        Debug.LogError("No axis selected for scaling.");
        return;
    }

    Vector3 currentControllerPosition = interactor.transform.position;
    Vector3 axisDirection = Vector3.zero;

    if (currentSelectedAxis == xAxis)
        axisDirection = Vector3.right;
    else if (currentSelectedAxis == yAxis)
        axisDirection = Vector3.up;
    else if (currentSelectedAxis == zAxis)
        axisDirection = Vector3.forward;

    Vector3 handDelta = currentControllerPosition - initialControllerPosition;
    float delta = -Vector3.Dot(handDelta, axisDirection);

    float maxAllowedScale = 5.0f; // Set the maximum allowed scale
    Vector3 newScale = targetObject.localScale;

    if (currentSelectedAxis == xAxis)
        newScale.x = Mathf.Clamp(initialScaleX.x + delta, 0.1f, maxAllowedScale);
    else if (currentSelectedAxis == yAxis)
        newScale.y = Mathf.Clamp(initialScaleY.y + delta, 0.1f, maxAllowedScale);
    else if (currentSelectedAxis == zAxis)
        newScale.z = Mathf.Clamp(initialScaleZ.z + delta, 0.1f, maxAllowedScale);

    targetObject.localScale = newScale;

    UpdateAxesPosition();
}

//     private void UpdateScaling()
// {

//     Debug.Log($"start update scaling!! {currentSelectedAxis}");
//     if (currentSelectedAxis == null)
//     {
//         Debug.LogError("No axis selected for scaling.");
//         return;
//     }

//     Debug.Log($"debuggggg1111");

//     // Get the current position of the controller and the object center
//     Vector3 currentControllerPosition = interactor.transform.position;
//     // Vector3 objectCenter = transform.position;

//     // Determine the direction vector based on the selected axis
//     Vector3 axisDirection = Vector3.zero;
//     Quaternion objectRotation = transform.rotation;

//     Debug.Log($"debuggggg22222");


//     if (currentSelectedAxis == xAxis)
//     {
//         axisDirection = objectRotation * Vector3.right; // Apply object rotation
//     }
//     else if (currentSelectedAxis == yAxis)
//     {
//         axisDirection = objectRotation * Vector3.up;
//     }
//     else if (currentSelectedAxis == zAxis)
//     {
//         axisDirection = objectRotation * Vector3.forward;
//     }

//     Debug.Log($"debuggggg3333");


//     // Calculate the hand delta along the selected axis direction
//     Vector3 handDelta = currentControllerPosition - initialControllerPosition;
//     float delta = -Vector3.Dot(handDelta, axisDirection); // Project onto the selected axis

//     Debug.Log($"delta is {delta}");
//     // Compute the new scale for the selected axis
//     float maxAllowedScale = 5.0f; // Set the maximum allowed scale (adjust as needed)

//     // Compute the new scale for the target object
//     Vector3 newScale = targetObject.localScale;

//     switch (currentSelectedAxis)
//     {
//         case var axis when axis == xAxis:
//             newScale.x = Mathf.Clamp(targetObject.localScale.x + delta, 0.1f, maxAllowedScale);
//             break;
//         case var axis when axis == yAxis:
//             newScale.y = Mathf.Clamp(targetObject.localScale.y + delta, 0.1f, maxAllowedScale);
//             break;
//         case var axis when axis == zAxis:
//             newScale.z = Mathf.Clamp(targetObject.localScale.z + delta, 0.1f, maxAllowedScale);
//             break;
//     }

//     targetObject.localScale = newScale;


//     UpdateAxesPosition();
// }


// Helper method to update axis positions based on object bounds
private void UpdateAxesPosition()
{
    Renderer objectRenderer = GetComponent<Renderer>();
    if (objectRenderer == null)
    {
        Debug.LogError("No Renderer found on the object. Cannot update axis positions.");
        return;
    }

    Bounds objectBounds = objectRenderer.bounds;

    if (xAxis != null)
        xAxis.position = objectBounds.center + objectBounds.extents.x * Vector3.right;
    if (yAxis != null)
        yAxis.position = objectBounds.center + objectBounds.extents.y * Vector3.up;
    if (zAxis != null)
        zAxis.position = objectBounds.center + objectBounds.extents.z * Vector3.forward;
}

    public void ClearAxes()
    {
        if (xAxis != null)
        {
            Destroy(xAxis.gameObject); // Destroy the axis GameObject
            xAxis = null; // Clear the reference
        }

        if (yAxis != null)
        {
            Destroy(yAxis.gameObject);
            yAxis = null;
        }

        if (zAxis != null)
        {
            Destroy(zAxis.gameObject);
            zAxis = null;
        }

        Debug.Log("Cleared all axes.");
    }

    public void SetTargetObject(Transform target)
  {
      targetObject = target;
      if (targetObject == null)
      {
          Debug.LogError("Target object is null! Ensure you assign it correctly.");
      }
      else
      {
          Debug.Log($"Target object set to: {targetObject.name}");
      }
  }

}
