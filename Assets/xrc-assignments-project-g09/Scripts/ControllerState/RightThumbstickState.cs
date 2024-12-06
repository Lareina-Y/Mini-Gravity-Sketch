using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace XRC.Assignments.Project.G09
{
    public class RightThumbstickState : MonoBehaviour
    {
        [SerializeField] private XRBaseInteractor m_Interactor;

        [SerializeField] private Transform defaultCenter;

        [SerializeField] private InputActionProperty m_RightThumbstickAction;

        [SerializeField] private Material sphereMaterial;

        [SerializeField] private Color feedbackColor = Color.blue; // Color when the thumbstick is pressed

        [SerializeField] private float defaultRadius = 0.015f;

        private GameObject feedbackBigSphere;

        private GameObject feedbackSmallSphere;

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
            if (feedbackBigSphere == null) // Only create the sphere if it doesn't already exist
            {
                // Create a GameObject for the feedback sphere
                feedbackBigSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                feedbackBigSphere.name = "FeedbackBigSphere";

                // Set the sphere's parent to the interactor
                feedbackBigSphere.transform.SetParent(defaultCenter);
                feedbackBigSphere.transform.localPosition = new Vector3(0, 0.01f, -0.015f);
                feedbackBigSphere.transform.localScale = Vector3.one * defaultRadius;
                feedbackBigSphere.transform.localRotation = defaultCenter.localRotation;

                // Set the sphere's material and color
                MeshRenderer bigSphereRenderer = feedbackBigSphere.GetComponent<MeshRenderer>();
                if (bigSphereRenderer != null)
                {
                    bigSphereRenderer.material = new Material(sphereMaterial);
                    bigSphereRenderer.material.color = feedbackColor;
                }
            }

            if (feedbackSmallSphere == null) // Only create the sphere if it doesn't already exist
            {
                // Create a GameObject for the feedback sphere
                feedbackSmallSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                feedbackSmallSphere.name = "FeedbackSmallSphere";

                // Set the sphere's parent to the interactor
                feedbackSmallSphere.transform.SetParent(defaultCenter);
                feedbackSmallSphere.transform.localPosition = new Vector3(0, 0.01f, 0.015f);
                feedbackSmallSphere.transform.localScale = Vector3.one * (defaultRadius / 2);
                feedbackSmallSphere.transform.localRotation = defaultCenter.localRotation;

                // Set the sphere's material and color
                MeshRenderer smallSphereRenderer = feedbackSmallSphere.GetComponent<MeshRenderer>();
                if (smallSphereRenderer != null)
                {
                    smallSphereRenderer.material = new Material(sphereMaterial);
                    smallSphereRenderer.material.color = feedbackColor;
                }
            }
        }

        // Called when the thumbstick is released
        private void OnRightSphereCanceled(InputAction.CallbackContext context)
        {
            if (feedbackBigSphere != null)
            {
                Destroy(feedbackBigSphere);
                feedbackBigSphere = null; // Reset the reference
            }

            if (feedbackSmallSphere != null)
            {
                Destroy(feedbackSmallSphere);
                feedbackSmallSphere = null; // Reset the reference
            }
        }
    }
}