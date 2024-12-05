using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.InputSystem;

public class DuplicateManager : MonoBehaviour
{
    [Header("References")]
    public XRDirectInteractor interactor;

    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference rightTriggered;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable currentInteractable;
    private bool isHolding = false;

    private void Start()
    {
        Debug.Log($"[DuplicateManager] Starting up. Interactor: {(interactor != null ? interactor.name : "null")}, RightTriggered: {(rightTriggered != null ? rightTriggered.name : "null")}");

        // Validate references
        if (interactor == null)
        {
            Debug.LogError("[DuplicateManager] XRDirectInteractor reference not set!");
            return;
        }
        if (rightTriggered == null)
        {
            Debug.LogError("[DuplicateManager] Right Trigger Input Action Reference not set!");
            return;
        }

        // Enable the input action
        rightTriggered.action.Enable();
        Debug.Log("[DuplicateManager] Input action enabled");

        // Subscribe to interactor events
        interactor.selectEntered.AddListener(OnGrab);
        interactor.selectExited.AddListener(OnRelease);
        Debug.Log("[DuplicateManager] Event listeners added");
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log($"[DuplicateManager] OnGrab called. Interactor: {args.interactorObject.transform.name}, Interactable: {args.interactableObject.transform.name}");
        isHolding = true;
        currentInteractable = args.interactableObject;
        Debug.Log($"[DuplicateManager] Grab state updated. IsHolding: {isHolding}, CurrentInteractable: {(currentInteractable != null ? currentInteractable.transform.name : "null")}");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log($"[DuplicateManager] OnRelease called. Was holding: {args.interactableObject.transform.name}");
        isHolding = false;
        currentInteractable = null;
        Debug.Log("[DuplicateManager] Release complete. All references cleared.");
    }

    private void Update()
    {
        if (isHolding && currentInteractable != null)
        {
            float triggerValue = rightTriggered.action.ReadValue<float>();

            if (rightTriggered.action.WasPressedThisFrame())
            {
                DuplicateObject();
            }
        }
    }

    private void DuplicateObject()
    {
        if (currentInteractable != null)
        {
            GameObject originalObject = currentInteractable.transform.gameObject;

            // Create a new instance
            Vector3 spawnPosition = originalObject.transform.position;
            Quaternion spawnRotation = originalObject.transform.rotation;

            GameObject newObject = Instantiate(originalObject, spawnPosition, spawnRotation);
        }
        else
        {
            Debug.LogWarning("[DuplicateManager] Attempted to duplicate but currentInteractable is null!");
        }
    }

    private void OnDestroy()
    {

        if (interactor != null)
        {
            interactor.selectEntered.RemoveListener(OnGrab);
            interactor.selectExited.RemoveListener(OnRelease);

        }

        if (rightTriggered != null)
        {
            rightTriggered.action.Disable();
        }
    }
}