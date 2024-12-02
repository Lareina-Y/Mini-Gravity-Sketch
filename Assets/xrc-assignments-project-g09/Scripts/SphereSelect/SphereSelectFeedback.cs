using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class SphereSelectFeedback : MonoBehaviour
{

    [SerializeField] private SphereSelectLogic sphereLogic;

    [Header("Visual Settings")]
    [SerializeField] private Material sphereMaterial;
    [SerializeField] private Color sphereNormalColor = new Color(0, 0.5f, 1f, 0.5f);
    [SerializeField] private Color sphereHoverColor = new Color(0, 1f, 0.5f, 0.5f);

    [SerializeField] private Color objectNormalColor = new Color(0.5f, 0.5f, 0.5f, 1);
    [SerializeField] private Color objectHoverColor = new Color(0.6f, 0.75f, 0.6f, 1);
    [SerializeField] private Color objectSelectColor = new Color(0.35f, 0.85f, 0.35f, 1);


    private List<MeshRenderer> selectedRenderers = new List<MeshRenderer>();
    private GameObject sphereIndicator;
    private MeshRenderer sphereRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake(){
        CreateSphereIndicator();
    }

    void Start(){

    }

    private void OnEnable()
    {
        sphereLogic.OnSphereUpdateEvent += UpdateSphereIndicator;
        sphereLogic.OnHoverEnteredEvent += OnHoverEntered;
        sphereLogic.OnHoverExitedEvent += OnHoverExited;
        sphereLogic.OnSelectEnteredEvent += OnSelectEntered;
        sphereLogic.OnSelectExitedEvent += OnSelectExited;
    }

    private void OnDisable()
    {
        sphereLogic.OnSphereUpdateEvent -= UpdateSphereIndicator;
        sphereLogic.OnHoverEnteredEvent -= OnHoverEntered;
        sphereLogic.OnHoverExitedEvent -= OnHoverExited;
        sphereLogic.OnSelectEnteredEvent -= OnSelectEntered;
        sphereLogic.OnSelectExitedEvent -= OnSelectExited;
    }

    private void UpdateSphereIndicator(Vector3 center, float radius)
    {
        sphereIndicator.transform.localPosition = center;
        sphereIndicator.transform.localScale = Vector3.one * radius * 2;
    }

    private void CreateSphereIndicator()
    {
        // Create a sphere and set it as a child of the default center
        sphereIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereIndicator.transform.parent = sphereLogic.DefaultCenter;

        // Reset position
        sphereIndicator.transform.localPosition = Vector3.zero;

        // Remove collider
        Destroy(sphereIndicator.GetComponent<Collider>());

        // Set material color
        sphereMaterial.SetColor("_BaseColor", sphereNormalColor);
        sphereRenderer = sphereIndicator.GetComponent<MeshRenderer>();
        sphereRenderer.material = sphereMaterial;
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        sphereMaterial.SetColor("_BaseColor", sphereHoverColor);

        // Set the color of the hovered object
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable hoverInteractable = (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable) args.interactableObject;
        MeshRenderer meshRenderer = hoverInteractable.transform.GetComponent<MeshRenderer>();
        meshRenderer.material.color = objectHoverColor;
    }

    private void OnHoverExited(bool hasHover, HoverExitEventArgs args)
    {

        // If no any hovered object, set the sphere color to the normal color
        if (!hasHover)
        {
            sphereMaterial.SetColor("_BaseColor", sphereNormalColor);
        }

        // Set the color of the hovered object
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable hoverInteractable = (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable) args.interactableObject;
        MeshRenderer meshRenderer = hoverInteractable.transform.GetComponent<MeshRenderer>();
        meshRenderer.material.color = objectNormalColor;
    }

    private void OnSelectEntered(List<MeshRenderer> selectedRenderers)
    {
        // Disable the sphere indicator
        sphereIndicator.SetActive(false);

        this.selectedRenderers = selectedRenderers;

        // Set the color of the selected object
        foreach (MeshRenderer meshRenderer in selectedRenderers)
        {
            meshRenderer.material.color = objectSelectColor;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Enable the sphere indicator
        sphereIndicator.SetActive(true);

        // When unselected, the object is still hovered, so set the color to the hover color
        sphereMaterial.SetColor("_BaseColor", sphereHoverColor);

        // Set the color of the selected object
        foreach (MeshRenderer meshRenderer in selectedRenderers)
        {
            meshRenderer.material.color = objectHoverColor;
        }
    }
}
