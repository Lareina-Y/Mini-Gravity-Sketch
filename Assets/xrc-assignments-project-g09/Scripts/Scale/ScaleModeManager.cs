using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ScaleModeManager : MonoBehaviour
{
    private bool isScaleMode = false;
    private bool isGripPressed = false;
    private XRGrabInteractable currentGrabbedObject;
    private Vector3 originalPosition;
    private Quaternion originalRotation; // Added to store rotation

    public GameObject xAxisPrefab;
    public GameObject yAxisPrefab;
    public GameObject zAxisPrefab;

    private GameObject xAxisControl;
    private GameObject yAxisControl;
    private GameObject zAxisControl;

    private XRGrabInteractable scalingObject;
    private AxisScaleController axisScaleController;

    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference leftYButtonAction;
    [SerializeField]
    private InputActionReference rightGripAction;

    [Header("References")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;

    void Start()
    {
        InitializeAxisScaleController();
        SetupInputActions();
        SetupInteractor();
    }

    private void SetupInputActions()
    {
        if (leftYButtonAction != null && rightGripAction != null)
        {
            leftYButtonAction.action.Enable();
            rightGripAction.action.Enable();
            leftYButtonAction.action.performed += OnScaleToggle;
            rightGripAction.action.performed += OnGripPressed;
            rightGripAction.action.canceled += OnGripReleased;
        }
    }

    private void SetupInteractor()
    {
        if (interactor != null)
        {
            interactor.selectEntered.AddListener(OnSelectEntered);
            interactor.selectExited.AddListener(OnSelectExited);
        }
    }

  private void InitializeAxisScaleController()
  {
      
      axisScaleController = GetComponent<AxisScaleController>();
      if (axisScaleController == null)
      {
          axisScaleController = gameObject.AddComponent<AxisScaleController>();
      }
     
  }
      

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (isScaleMode) return;

        currentGrabbedObject = args.interactableObject.transform.GetComponent<XRGrabInteractable>();
        if (currentGrabbedObject != null)
        {
            originalPosition = currentGrabbedObject.transform.position;
            originalRotation = currentGrabbedObject.transform.rotation; 
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (isScaleMode) return;

        currentGrabbedObject = null;
        CleanupAxes();
    }

private void OnScaleToggle(InputAction.CallbackContext context)
{
    Debug.Log("Left Y Button Pressed!");
    Debug.Log($"Current Scale Mode: {isScaleMode}, Grip State: {isGripPressed}");
  
    if (!isScaleMode)
    {
        // Attempt to enter Scale Mode
        if (isGripPressed && currentGrabbedObject != null)
        {
            isScaleMode = true;
            axisScaleController.IsInScaleMode = true;

            scalingObject = currentGrabbedObject;

            // Disable object tracking and reset position/rotation
            scalingObject.trackPosition = false;
            scalingObject.trackRotation = false;
            scalingObject.transform.position = originalPosition;
            scalingObject.transform.rotation = originalRotation;

            Debug.Log($"Entered Scale Mode: Object reset to original position: {originalPosition} and rotation: {originalRotation.eulerAngles}");

            axisScaleController.SetTargetObject(scalingObject.transform);

            // Retrieve object size and create control axes
            Renderer objectRenderer = scalingObject.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                Vector3 objectSize = objectRenderer.bounds.size;
                CreateAxes(objectSize); // Add this call to create the axes
            }
        }
    }
    else
    {
        // Exit Scale Mode
        isScaleMode = false;
        axisScaleController.IsInScaleMode = false;

        if (scalingObject != null)
        {
            // Re-enable object tracking
            scalingObject.trackPosition = true;
            scalingObject.trackRotation = true;
            scalingObject = null;
        }
        else
        {
            // Log if there is no scaling object during exit
            Debug.LogWarning("Exited Scale Mode, but no scaling object was found.");
        }

        // Clean up control axes
        CleanupAxes();
    }
}



    private void OnGripPressed(InputAction.CallbackContext context)
    {
        Debug.LogWarning("OnGripPressed!!!!!!");
        isGripPressed = true;
    }

    private void OnGripReleased(InputAction.CallbackContext context)
    {
        Debug.LogWarning("OnGripReleased!!!!!!");
        isGripPressed = false;
    }

    private void CreateAxes(Vector3 size)
    {
        Debug.Log($"Start creating axes!");

        // Instantiate and configure X Axis
        xAxisControl = Instantiate(xAxisPrefab, currentGrabbedObject.transform);
        ConfigureAxis(xAxisControl, "X");
        InitializeAxisCollisionHandler(xAxisControl);

        // Instantiate and configure Y Axis
        yAxisControl = Instantiate(yAxisPrefab, currentGrabbedObject.transform);
        ConfigureAxis(yAxisControl, "Y");
        InitializeAxisCollisionHandler(yAxisControl);

        // Instantiate and configure Z Axis
        zAxisControl = Instantiate(zAxisPrefab, currentGrabbedObject.transform);
        ConfigureAxis(zAxisControl, "Z");
        InitializeAxisCollisionHandler(zAxisControl);

        if (axisScaleController != null)
        {
            axisScaleController.SetAxes(xAxisControl.transform, yAxisControl.transform, zAxisControl.transform);
        }
        else
        {
            Debug.LogError("AxisScaleController not found! Axes assignment skipped.");
        }


        Debug.Log("Custom axis controllers created and initialized using prefabs.");
    }


    private void InitializeAxisCollisionHandler(GameObject axisControl)
    {
        
        AxisCollisionHandler collisionHandler = axisControl.GetComponent<AxisCollisionHandler>();
        Debug.Log($" Debug the InitializeAxisCollisionHandler {collisionHandler != null}");
        if (collisionHandler != null)
        {
            collisionHandler.Initialize(axisScaleController);
            Debug.Log($"AxisCollisionHandler initialized for {axisControl.name}");
        }
        else
        {
            Debug.LogWarning($"AxisCollisionHandler not found on {axisControl.name}. Ensure the prefab has this component.");
        }
    }


    private void ConfigureAxis(GameObject axis, string axisName)
    {
        Vector3 positionOffset = Vector3.zero;
        Quaternion rotationOffset = Quaternion.identity;

        switch (axisName)
        {
            case "X":
                axis.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                positionOffset = new Vector3(0.784f, 0, 0);
                rotationOffset = Quaternion.identity;
                break;

            case "Y":
                axis.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                positionOffset = new Vector3(0, 0.83f, 0);
                rotationOffset = Quaternion.Euler(0, 0, 90);
                break;

            case "Z":
                axis.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                positionOffset = new Vector3(0, 0, 0.796f);
                rotationOffset = Quaternion.Euler(0, 270, 0);
                break;
        }

        axis.transform.localPosition = positionOffset;
        axis.transform.localRotation = rotationOffset;

        Debug.Log($"{axisName} axis configured with position {positionOffset}, rotation {rotationOffset.eulerAngles}, and scale {axis.transform.localScale}");
    }

    private void CleanupAxes()
    {
        if (xAxisControl != null) Destroy(xAxisControl);
        if (yAxisControl != null) Destroy(yAxisControl);
        if (zAxisControl != null) Destroy(zAxisControl);

        xAxisControl = null;
        yAxisControl = null;
        zAxisControl = null;

    }

    private void OnDestroy()
    {
        CleanupInputActions();
        CleanupInteractor();
        CleanupAxes();
    }

    private void CleanupInputActions()
    {
        if (leftYButtonAction != null)
        {
            leftYButtonAction.action.performed -= OnScaleToggle;
            leftYButtonAction.action.Disable();
        }
        if (rightGripAction != null)
        {
            rightGripAction.action.performed -= OnGripPressed;
            rightGripAction.action.canceled -= OnGripReleased;
            rightGripAction.action.Disable();
        }
    }

    private void CleanupInteractor()
    {
        if (interactor != null)
        {
            interactor.selectEntered.RemoveListener(OnSelectEntered);
            interactor.selectExited.RemoveListener(OnSelectExited);
        }
    }
}
