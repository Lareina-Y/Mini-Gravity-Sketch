using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class ScaleModeManager : MonoBehaviour
{
    private bool isScaleMode = false;
    private bool isGripPressed = false;
    private XRGrabInteractable currentGrabbedObject;
    private Vector3 originalPosition;
    private XRGrabInteractable scalingObject;
    private Quaternion originalRotation;

    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference leftYButtonAction;
    [SerializeField]
    private InputActionReference rightGripAction;

    [Header("References")]
    public XRDirectInteractor interactor;

    void Start()
    {
        Debug.Log("ScaleModeManager Starting...");
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

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (isScaleMode){
          return;
        }
        
        currentGrabbedObject = args.interactableObject.transform.GetComponent<XRGrabInteractable>();
        if (currentGrabbedObject != null)
        {
            originalPosition = currentGrabbedObject.transform.position;
            originalRotation = currentGrabbedObject.transform.rotation;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (isScaleMode){
          return;
        }
        currentGrabbedObject = null;
    }

    private void OnScaleToggle(InputAction.CallbackContext context)
    {
        Debug.Log("Left Y pressed!");
        Debug.Log($"Current Scale Mode: {isScaleMode}, Grip State: {isGripPressed}");

        if (!isScaleMode)
        {
            if (isGripPressed && currentGrabbedObject != null)
            {
                isScaleMode = true;
                AxisScaleController.IsInScaleMode = true;

                scalingObject = currentGrabbedObject;

                scalingObject.trackPosition = false;
                scalingObject.trackRotation = false;
                scalingObject.transform.position = originalPosition;
                scalingObject.transform.rotation = originalRotation;
                Debug.Log($"Entered Scale Mode, object returned to original position: {originalPosition}");
            }
            else
            {
                Debug.Log("Cannot enter Scale Mode - No object grabbed or grip not pressed");
            }
        }
        else
        {
            isScaleMode = false;
            AxisScaleController.IsInScaleMode = false;


            if (scalingObject != null)
            {
                Debug.Log("Reset the object position");
                scalingObject.trackPosition = true;
                scalingObject.trackRotation = true;
                scalingObject = null;
            }
            else {
                Debug.Log("currentGrabbedObject is empty!");
            }
            Debug.Log("Exited Scale Mode, grab enabled");
        }
    }

    private void OnGripPressed(InputAction.CallbackContext context)
    {
        isGripPressed = true;
        
    }

    private void OnGripReleased(InputAction.CallbackContext context)
    {
        isGripPressed = false;
    }

    void OnDestroy()
    {
        CleanupInputActions();
        CleanupInteractor();
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
