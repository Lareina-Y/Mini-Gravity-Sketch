using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SphereSelectFeedback : MonoBehaviour
{
    [SerializeField] public Material sphereMaterial;
    [SerializeField] public Color defaultColor = Color.grey;
    [SerializeField] public Color hoverColor = Color.red;

    private SphereSelectLogic _sphereSelectLogic;
    private MeshRenderer sphereRenderer;
    private Transform sphereTransform;
    
    private void Awake()
    {
        _sphereSelectLogic = GetComponent<SphereSelectLogic>();
        CreateFeedbackSphere();
    }

    private void OnEnable()
    {
        if (_sphereSelectLogic != null && _sphereSelectLogic.interactor != null)
        {
            // Add listeners to interactor events
            _sphereSelectLogic.interactor.hoverEntered.AddListener(OnHoverEntered);
            _sphereSelectLogic.interactor.hoverExited.AddListener(OnHoverExited);
            _sphereSelectLogic.interactor.selectEntered.AddListener(OnSelectEntered);
            _sphereSelectLogic.interactor.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        if (_sphereSelectLogic != null && _sphereSelectLogic.interactor != null)
        {
            // Remove listeners from interactor events
            _sphereSelectLogic.interactor.hoverEntered.RemoveListener(OnHoverEntered);
            _sphereSelectLogic.interactor.hoverExited.RemoveListener(OnHoverExited);
            _sphereSelectLogic.interactor.selectEntered.RemoveListener(OnSelectEntered);
            _sphereSelectLogic.interactor.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void Update()
    {
        Vector3 sphereCenter = _sphereSelectLogic.GetSphereCollider().center;
        sphereTransform.position = sphereCenter;
        sphereTransform.localScale = Vector3.one * (_sphereSelectLogic.GetCurrentRadius() * 2); // Diameter of sphere
    }

    private void CreateFeedbackSphere()
    {
        // Create a GameObject for the feedback sphere
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "FeedbackSphere";
        sphereTransform = sphere.transform;

        // Set the sphere's parent to the current GameObject
        sphereTransform.SetParent(transform);
        sphereTransform.localPosition = Vector3.zero;
        sphereTransform.localScale = Vector3.one * _sphereSelectLogic.defaultRadius * 2; // Diameter of default sphere

        // Get the renderer and assign the material
        sphereRenderer = sphere.GetComponent<MeshRenderer>();
        if (sphereRenderer != null)
        {
            sphereRenderer.material = new Material(sphereMaterial);
            sphereRenderer.material.color = defaultColor;
        }

        // Remove the sphere's collider since it's for visual feedback
        Destroy(sphere.GetComponent<Collider>());
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        // Change the sphere's color to the hover color
        if (sphereRenderer != null)
        {
            sphereRenderer.material.color = hoverColor;
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        // Reset the sphere's color to the default color
        if (sphereRenderer != null)
        {
            sphereRenderer.material.color = defaultColor;
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Hide the sphere when the interactor is selecting an object
        if (sphereRenderer != null)
        {
            sphereRenderer.enabled = false;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Show the sphere when the selection ends
        if (sphereRenderer != null)
        {
            sphereRenderer.enabled = true;
        }
    }
}

