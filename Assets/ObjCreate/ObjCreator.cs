using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjCreator : MonoBehaviour
{
    public GameObject objectPrefab;
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;
    public float minScale = 0.1f;
    public float maxScale = 5f;
    private float previousScale;
    public float scaleThreshold = 0.01f; // 1cm threshold

    private GameObject previewObject;
    private bool isCreating = false;

    private InputAction triggerAction;

    private void Awake()
    {
        // Set up the input action for the trigger - Changed to RightHand
        triggerAction = new InputAction("Trigger", InputActionType.Button);
        triggerAction.AddBinding("<XRController>{RightHand}/triggerPressed");
        triggerAction.performed += ctx => StartCreatingObject();
        triggerAction.canceled += ctx => ConfirmObject();
        triggerAction.Enable();
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
        isCreating = true;

        // Calculate initial position and scale
        Vector3 midPoint = Vector3.Lerp(leftControllerTransform.position, rightControllerTransform.position, 0.5f);
        float initialDistance = Vector3.Distance(leftControllerTransform.position, rightControllerTransform.position);
        float initialScale = Mathf.Clamp(initialDistance, minScale, maxScale);

        previousScale = initialScale;

        // Create and initialize the preview object
        previewObject = Instantiate(objectPrefab, midPoint, Quaternion.identity);
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
        isCreating = false;

        if (previewObject != null)
        {
            // Make the object fully opaque
            previewObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            previewObject = null;
        }
    }
}