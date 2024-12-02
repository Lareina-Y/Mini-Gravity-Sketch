using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RightThumbstickState : MonoBehaviour
{
    [SerializeField]
    private XRBaseInteractor m_Interactor;
    
    [SerializeField] 
    private Transform defaultCenter;
    
    [SerializeField]
    private InputActionProperty m_RightThumbstickAction;
    
    [SerializeField] 
    private Material sphereMaterial;
    
    [SerializeField]
    private Color feedbackColor = Color.blue; // Color when the thumbstick is pressed
    
    [SerializeField]
    private float defaultRadius = 0.01f;
    
    private GameObject feedbackSphere;
    
    // Enable the input action
    protected void OnEnable()
    {
        m_RightThumbstickAction.action.Enable();
    }

    // Disable the input action
    protected void OnDisable()
    {
        m_RightThumbstickAction.action.Disable();
    }

    // Initialize the input events
    private void Start()
    {
        // Listen for thumbstick action input
        m_RightThumbstickAction.action.performed += OnRightSpherePerformed;
        m_RightThumbstickAction.action.canceled += OnRightSphereCanceled;
    }

    // Called when the thumbstick is pressed
    private void OnRightSpherePerformed(InputAction.CallbackContext context)
    {
        if (feedbackSphere == null) // Only create the sphere if it doesn't already exist
        {
            // Create a GameObject for the feedback sphere
            feedbackSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            feedbackSphere.name = "FeedbackSphere";

            // Set the sphere's parent to the interactor
            feedbackSphere.transform.SetParent(defaultCenter);
            feedbackSphere.transform.localPosition = new Vector3(0, 0.01f, -0.01f);
            feedbackSphere.transform.localScale = Vector3.one * defaultRadius;
            feedbackSphere.transform.localRotation = Quaternion.Euler(-30f, 0f, 0f);

            // Set the sphere's material and color
            MeshRenderer sphereRenderer = feedbackSphere.GetComponent<MeshRenderer>();
            if (sphereRenderer != null)
            {
                sphereRenderer.material = new Material(sphereMaterial);
                sphereRenderer.material.color = feedbackColor;
            }
        }
    }

    // Called when the thumbstick is released
    private void OnRightSphereCanceled(InputAction.CallbackContext context)
    {
        if (feedbackSphere != null)
        {
            Destroy(feedbackSphere);
            feedbackSphere = null; // Reset the reference
        }
    }
}
