using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectObjFeedback : MonoBehaviour
{
    public Material sphereMaterial;
    public Color defaultColor = Color.grey;
    public Color hoverColor = Color.red;

    private SelectObjLogic _selectObjLogic;
    private MeshRenderer sphereRenderer;
    private Transform sphereTransform;

    private void Awake()
    {
        _selectObjLogic = GetComponent<SelectObjLogic>();
        CreateFeedbackSphere();
    }

    private void OnEnable()
    {
        if (_selectObjLogic != null && _selectObjLogic.interactor != null)
        {
            // Add listeners to interactor events
            _selectObjLogic.interactor.hoverEntered.AddListener(OnHoverEntered);
            _selectObjLogic.interactor.hoverExited.AddListener(OnHoverExited);
            _selectObjLogic.interactor.selectEntered.AddListener(OnSelectEntered);
            _selectObjLogic.interactor.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        if (_selectObjLogic != null && _selectObjLogic.interactor != null)
        {
            // Remove listeners from interactor events
            _selectObjLogic.interactor.hoverEntered.RemoveListener(OnHoverEntered);
            _selectObjLogic.interactor.hoverExited.RemoveListener(OnHoverExited);
            _selectObjLogic.interactor.selectEntered.RemoveListener(OnSelectEntered);
            _selectObjLogic.interactor.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void Update()
    {
        if (sphereTransform is not null)
        {
            sphereTransform.position = _selectObjLogic.defaultCenter.position;
            sphereTransform.localScale = Vector3.one * (_selectObjLogic.getCurrentRadius() * 2); // Diameter of sphere
        }
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
        sphereTransform.localScale = Vector3.one * _selectObjLogic.defaultRadius * 2; // Diameter of default sphere

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

