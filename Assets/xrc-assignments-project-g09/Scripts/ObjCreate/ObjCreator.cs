using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.XR.Interaction.Toolkit;

using SetShape;

public class ObjCreator : MonoBehaviour
{
    [SerializeField] private SetShapeLogic setShapeLogic;
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;
    public float minScale = 0.1f;
    public float maxScale = 5f;
    private float previousScale;
    public float scaleThreshold = 0.01f; // 1cm threshold

    private GameObject previewObject;
    private bool isCreating = false;

    private InputAction triggerAction;

    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor m_Interactor;

    private void Awake()
    {
        // Set up the input action for the trigger - Changed to RightHand
        triggerAction = new InputAction("Trigger", InputActionType.Button);
        triggerAction.AddBinding("<XRController>{RightHand}/triggerPressed");
        triggerAction.performed += ctx => StartCreatingObject();
        triggerAction.canceled += ctx => ConfirmObject();
        triggerAction.Enable();
    }

    private void Start()
    {
        setShapeLogic ??=  Object.FindFirstObjectByType<SetShapeLogic>();
    }

    private void OnDisable()
    {
        triggerAction.Disable();
    }

    private void Update()
    {
        if (isCreating)
        {
            UpdateObjectScale();
        }
    }

    private void StartCreatingObject()
    {
        // Check if we're currently selecting an object
        if (m_Interactor.hasSelection)
        {
            // Disable grabbing of the original object
            IXRSelectInteractable currentInteractable = m_Interactor.firstInteractableSelected;

            // m_Interactor.interactionManager.CancelInteractableSelection(currentInteractable);

            // // Ensure the original object is no longer selected
            m_Interactor.interactionManager.SelectExit(m_Interactor, currentInteractable);
            m_Interactor.interactionManager.CancelInteractableSelection(currentInteractable);

            // Make a duplication of the currently grabbed object
            GameObject currentObject = currentInteractable.transform.gameObject;
            GameObject duplicatedObject = GameObject.Instantiate(currentObject);
            SetInteractable(duplicatedObject);
            IXRSelectInteractable duplicatedInteractable = duplicatedObject.GetComponent<IXRSelectInteractable>();



            // Force the interactor to grab the duplicate object
            m_Interactor.interactionManager.SelectEnter(m_Interactor, duplicatedInteractable);
            m_Interactor.interactionManager.CancelInteractableSelection(currentInteractable);
            // m_Interactor.interactionManager.SelectExit(m_Interactor, currentInteractable);

            return;
        }

        // If no object is selected, create a new one (existing creation logic)
        isCreating = true;

        GameObject prefabToCreate = setShapeLogic.GetCurrentShapePrefab();

        SetInteractable(prefabToCreate);

        if (prefabToCreate == null)
        {
            Debug.LogError("Object Creator: No prefab assigned for the current shape!");
            return;
        }

        // Calculate initial position and scale
        Vector3 midPoint = Vector3.Lerp(leftControllerTransform.position, rightControllerTransform.position, 0.5f);
        float initialDistance = Vector3.Distance(leftControllerTransform.position, rightControllerTransform.position);
        float initialScale = Mathf.Clamp(initialDistance, minScale, maxScale);

        previousScale = initialScale;

        // Create and initialize the preview object
        previewObject = Instantiate(prefabToCreate, midPoint, Quaternion.identity);
        previewObject.transform.localScale = Vector3.one * initialScale;
        previewObject.transform.LookAt(Camera.main.transform);
        previewObject.transform.eulerAngles = new Vector3(0, previewObject.transform.eulerAngles.y, 0);


        // Make the preview object transparent
        previewObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.75f);
    }

    private void TriggerHapticFeedback()
    {
        // For left controller
        UnityEngine.XR.InputDevice leftDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand);
        if (leftDevice.isValid)
        {
            leftDevice.SendHapticImpulse(0, 0.5f, 0.01f);
        }

        // For right controller
        UnityEngine.XR.InputDevice rightDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
        if (rightDevice.isValid)
        {
            rightDevice.SendHapticImpulse(0, 0.5f, 0.01f);
        }
    }

    private void UpdateObjectScale()
    {
        if (previewObject != null)
        {
            float distance = Vector3.Distance(leftControllerTransform.position, rightControllerTransform.position);
            float scale = Mathf.Clamp(distance, minScale, maxScale);

            previewObject.transform.localScale = Vector3.one * scale;
            previewObject.transform.position = Vector3.Lerp(leftControllerTransform.position, rightControllerTransform.position, 0.5f);

            if (Mathf.Abs(scale - previousScale) >= scaleThreshold)
            {
                TriggerHapticFeedback();
                previousScale = scale;
            }
        }
    }

    private void ConfirmObject()
    {
        if (!isCreating)
        {
            return;
        }

        isCreating = false;

        if (previewObject != null)
        {
            // Make the object fully opaque
            Color currColor = ChangeColorLogic.Instance.Color;
            
            previewObject.GetComponent<MeshRenderer>().material.color = currColor;

            previewObject = null;
        }
    }

    private void SetInteractable(GameObject createdObject)
    {
        XRGrabInteractable interactable = createdObject.GetComponent<XRGrabInteractable>();

        if (interactable == null)
        {
            interactable = createdObject.AddComponent<XRGrabInteractable>();
        }

        interactable.useDynamicAttach = true;
        interactable.matchAttachPosition = true;
        interactable.matchAttachRotation = true;
        interactable.snapToColliderVolume = false;
        interactable.throwOnDetach = false;

        // Ensure the XRGeneralGrabTransformer is added to the GameObject, not the interactable
        XRGeneralGrabTransformer transformer = createdObject.GetComponent<XRGeneralGrabTransformer>();
        if (transformer == null)
        {
            transformer = createdObject.AddComponent<XRGeneralGrabTransformer>();
        }
        interactable.AddMultipleGrabTransformer(transformer);

        Rigidbody rb = createdObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = createdObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
    }
}