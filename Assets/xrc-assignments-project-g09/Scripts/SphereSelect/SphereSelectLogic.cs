using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using MeshManipulation;
using MeshManipulation.UI;
using System.Collections.Generic;

public class SphereSelectLogic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRBaseInteractor interactor;
    [SerializeField] private Transform defaultCenter;
    [SerializeField] private MeshManipulationLogic meshManipulationLogic;

    [Header("Sphere Radius Settings")]
    [SerializeField] private float minRadius = 0.01f;
    [SerializeField] private float maxRadius = 0.3f;
    [SerializeField] private float defaultRadius = 0.04f;
    [SerializeField] private float vertexSelectionThreshold = 0.05f;
    [SerializeField] private float edgeSelectionThreshold = 0.05f;
    [SerializeField] private float faceSelectionThreshold = 0.05f;

    private SphereCollider sphereCollider;
    private Vector3 sphereColliderOffset;
    private float currentRadius;
    private Vector3 sphereOffset;
    private MeshSelectionUI.SelectionMode currentMode = MeshSelectionUI.SelectionMode.Object;

    // Properties
    public float CurrentRadius => currentRadius;
    public float DefaultRadius => defaultRadius;
    public Transform DefaultCenter => defaultCenter;

    // Events
    public delegate void OnSphereUpdate(Vector3 center, float radius);
    public event OnSphereUpdate OnSphereUpdateEvent;
    public event System.Action<HoverEnterEventArgs> OnHoverEnteredEvent;
    public event System.Action<bool, HoverExitEventArgs> OnHoverExitedEvent;
    public event System.Action<List<MeshRenderer>> OnSelectEnteredEvent;
    public event System.Action<SelectExitEventArgs> OnSelectExitedEvent;

    // Selection Events
    public event System.Action<GameObject> OnObjectSelected;
    public event System.Action<int[]> OnVerticesSelected;
    public event System.Action<int[]> OnEdgesSelected;
    public event System.Action<int[]> OnFacesSelected;

    void Awake()
    {
        if (interactor == null)
        {
            interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        }
        
        sphereCollider = interactor.GetComponent<SphereCollider>();

        if (defaultCenter == null)
        {
            Debug.LogError("Default center transform is not assigned.");
        }
        
        currentRadius = defaultRadius;
        sphereCollider.radius = currentRadius;
        sphereCollider.isTrigger = true;

        Vector3 offset = defaultCenter.position - interactor.transform.position;
        sphereColliderOffset = interactor.transform.InverseTransformDirection(offset);
    }
    
    void Start()
    {
        
        if (meshManipulationLogic != null)
        {
            meshManipulationLogic.OnModeChanged += HandleModeChange;
        }

        interactor.hoverEntered.AddListener(OnHoverEntered);
        interactor.hoverExited.AddListener(OnHoverExited);
        interactor.selectEntered.AddListener(OnSelectEntered);
        interactor.selectExited.AddListener(OnSelectExited);

        UpdateSpherePosition();
    }

    void OnDestroy()
    {
        if (meshManipulationLogic != null)
        {
            meshManipulationLogic.OnModeChanged -= HandleModeChange;
        }

        if (interactor != null)
        {
            interactor.hoverEntered.RemoveListener(OnHoverEntered);
            interactor.hoverExited.RemoveListener(OnHoverExited);
            interactor.selectEntered.RemoveListener(OnSelectEntered);
            interactor.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void HandleModeChange(MeshSelectionUI.SelectionMode newMode)
    {
        currentMode = newMode;
    }

    private void Update()
    {
        UpdateSpherePosition();
    }

    private void UpdateSpherePosition()
    {
        if (currentRadius < defaultRadius)
        {
            // When the radius is less than the default radius, offset the sphere
            float offset = defaultRadius - currentRadius;
            sphereOffset = new Vector3(-offset, 0, 0);
        }
        else
        {
            // When the radius is at the default radius, no offset is applied
            sphereOffset = Vector3.zero;
        }

        sphereCollider.center = sphereColliderOffset + sphereOffset;
        interactor.attachTransform.localPosition = sphereColliderOffset + sphereOffset;

        OnSphereUpdateEvent?.Invoke(sphereOffset, currentRadius);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        OnHoverEnteredEvent?.Invoke(args);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        IXRHoverInteractor hoverInteractor = (IXRHoverInteractor)interactor;
        OnHoverExitedEvent?.Invoke(hoverInteractor.hasHover, args);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Get collider position in world space
        Vector3 colliderWorldPosition = interactor.transform.TransformPoint(sphereCollider.center);

        switch (currentMode)
        {
            case MeshSelectionUI.SelectionMode.Object:
                SelectObjects(colliderWorldPosition);
                break;
            case MeshSelectionUI.SelectionMode.Vertex:
                SelectVertices(colliderWorldPosition);
                break;
            case MeshSelectionUI.SelectionMode.Edge:
                SelectEdges(colliderWorldPosition);
                break;
            case MeshSelectionUI.SelectionMode.Face:
                SelectFaces(colliderWorldPosition);
                break;
        }
    }

    private void SelectObjects(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, sphereCollider.radius);
        List<MeshRenderer> selectedRenderers = new List<MeshRenderer>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out XRBaseInteractable interactable))
            {   
                IXRSelectInteractable selectInteractable = (IXRSelectInteractable)interactable;
                if (selectInteractable.transform.TryGetComponent(out MeshRenderer meshRenderer))
                {
                    selectedRenderers.Add(meshRenderer);
                }
                
                interactor.interactionManager.SelectEnter(interactor, selectInteractable);
                OnObjectSelected?.Invoke(selectInteractable.transform.gameObject);
            }
        }

        OnSelectEnteredEvent?.Invoke(selectedRenderers);
    }

    private void SelectVertices(Vector3 position)
    {
        // TODO: Implement vertex selection logic using sphere overlap
        // This will be implemented in the next step
    }

    private void SelectEdges(Vector3 position)
    {
        // TODO: Implement edge selection logic using sphere overlap
        // This will be implemented in the next step
    }

    private void SelectFaces(Vector3 position)
    {
        // TODO: Implement face selection logic using sphere overlap
        // This will be implemented in the next step
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        OnSelectExitedEvent?.Invoke(args);
    }

    public void AdjustRadius(float change)
    {
        currentRadius = Mathf.Clamp(currentRadius + change, minRadius, maxRadius);
        sphereCollider.radius = currentRadius;
        UpdateSpherePosition();
    }

    public SphereCollider GetSphereCollider()
    {
        return sphereCollider;
    }
}