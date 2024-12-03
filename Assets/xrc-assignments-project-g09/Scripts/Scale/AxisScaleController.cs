using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class AxisScaleController : MonoBehaviour
{
    private Transform xAxis;
    private Transform yAxis;
    private Transform zAxis;
    private Transform axisParent;  
    private static AxisScaleController Instance;  

    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference rightGripAction;

    // Current Scaling State
    private Transform currentSelectedAxis;
    private Vector3 initialControllerPosition;
    private Vector3 initialScale;
    private bool isScaling = false;

    // Scale Mode State
    private static bool isInScaleMode = false;
    public static bool IsInScaleMode
    {
        get { return isInScaleMode; }
        set
        {
            isInScaleMode = value;
            Debug.Log($"Scale Mode changed to: {value}");

            if (Instance != null)
            {
                Instance.UpdateAxisVisibility(value);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

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
        axisParent = transform.Find("Axis");
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

        // 初始化时隐藏轴
        UpdateAxisVisibility(false);

        SetupAxisCollider(xAxis);
        SetupAxisCollider(yAxis);
        SetupAxisCollider(zAxis);
    }

    private void UpdateAxisVisibility(bool visible)
    {
        if (axisParent != null)
        {
            axisParent.gameObject.SetActive(visible);
            Debug.Log($"Axis visibility set to: {visible}");
        }
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

    private void StopScaling()
    {
        isScaling = false;
        currentSelectedAxis = null;
        Debug.Log("Stopped scaling.");
    }

    private void Update()
    {
        if (isScaling && currentSelectedAxis != null)
        {
            UpdateScaling();
        }
    }

    private void UpdateScaling()
{
    Vector3 controllerPosition = currentSelectedAxis.position; 
    Vector3 objectCenter = transform.position;

    Vector3 directionToController = controllerPosition - objectCenter;

    Vector3 projectionAxis = Vector3.right; 
    if (currentSelectedAxis == yAxis)
        projectionAxis = Vector3.up;
    else if (currentSelectedAxis == zAxis)
        projectionAxis = Vector3.forward;

    // 4. 计算投影长度
    float projectedDistance = Vector3.Dot(directionToController, projectionAxis);
    
    // 5. 计算新的缩放值（减去初始距离得到delta）
    float initialDistance = Vector3.Dot(initialControllerPosition - objectCenter, projectionAxis);
    float scaleDelta = projectedDistance - initialDistance;

    // 6. 应用新的缩放值
    Vector3 newScale = initialScale;
    if (currentSelectedAxis == xAxis)
        newScale.x = Mathf.Max(0.1f, initialScale.x + scaleDelta);
    else if (currentSelectedAxis == yAxis)
        newScale.y = Mathf.Max(0.1f, initialScale.y + scaleDelta);
    else if (currentSelectedAxis == zAxis)
        newScale.z = Mathf.Max(0.1f, initialScale.z + scaleDelta);

    // 7. 更新物体缩放
    transform.localScale = newScale;

    Debug.Log($"Scaling - Distance: {projectedDistance:F2}, Delta: {scaleDelta:F2}, New Scale: {newScale}");
}
}
