using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using MeshManipulation;
using MeshManipulation.UI;
using System.Collections.Generic;
using UndoRedo.Core;
using UndoRedo.Commands;
using UnityEngine.InputSystem;

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

    [SerializeField] private InputActionProperty undoAction;
    [SerializeField] private InputActionProperty redoAction;

    private Transform selectedObjectTransform;
    private Vector3 initialPosition;

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

        if (defaultCenter == null)
        {
            Debug.LogError("Default center transform is not assigned.");
        }
    }
    
    void Start()
    {

        // Make sure the UndoRedoManager is in the scene
        if (UndoRedoManager.Instance == null)
        {
            Debug.LogError("UndoRedoManager is not in the scene. Please add it to the scene.");
        }

        sphereCollider = interactor.GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;


        Vector3 offset = defaultCenter.position - interactor.transform.position;
        sphereColliderOffset = interactor.transform.InverseTransformDirection(offset);

        // Add corresponding interaction layers based on the current mode
        if (interactor is XRDirectInteractor directInteractor)
        {
            var currentLayers = directInteractor.interactionLayers.value;
            switch (meshManipulationLogic.GetCurrentMode())
            {
                case MeshSelectionUI.SelectionMode.Object:
                    currentLayers |= InteractionLayerMask.GetMask("SphereSelectObject");
                    break;
                case MeshSelectionUI.SelectionMode.Vertex:
                    currentLayers |= InteractionLayerMask.GetMask("SphereSelectVertex");
                    break;
                case MeshSelectionUI.SelectionMode.Edge:
                    currentLayers |= InteractionLayerMask.GetMask("SphereSelectEdge");
                    break;
                case MeshSelectionUI.SelectionMode.Face:
                    currentLayers |= InteractionLayerMask.GetMask("SphereSelectFace");
                    break;
            }
            directInteractor.interactionLayers = currentLayers;
        }

        currentRadius = defaultRadius;
        sphereCollider.radius = currentRadius;
        UpdateSpherePosition();

        if (meshManipulationLogic == null)
        {
            meshManipulationLogic = FindObjectOfType<MeshManipulationLogic>();
        }
        
        if (meshManipulationLogic != null)
        {
            meshManipulationLogic.OnModeChanged += HandleModeChange;
        }

        interactor.hoverEntered.AddListener(OnHoverEntered);
        interactor.hoverExited.AddListener(OnHoverExited);
        interactor.selectEntered.AddListener(OnSelectEntered);
        interactor.selectExited.AddListener(OnSelectExited);

        OnEnable();
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

        OnDisable();
    }

    private void HandleModeChange(MeshSelectionUI.SelectionMode newMode)
    {
        currentMode = newMode;
        
        if (interactor is XRDirectInteractor directInteractor)
        {
            // Get current interaction layers
            var currentLayers = directInteractor.interactionLayers.value;
            
            // Remove all SphereSelect related layers
            currentLayers &= ~InteractionLayerMask.GetMask(
                "SphereSelectObject",
                "SphereSelectVertex",
                "SphereSelectEdge",
                "SphereSelectFace"
            );
            
            // Add corresponding layers based on the new mode
            switch (newMode)
            {
                case MeshSelectionUI.SelectionMode.Object:
                    currentLayers |= InteractionLayerMask.GetMask("SphereSelectObject");
                    break;
                case MeshSelectionUI.SelectionMode.Vertex:
                    currentLayers |= InteractionLayerMask.GetMask("SphereSelectVertex");
                    break;
                case MeshSelectionUI.SelectionMode.Edge:
                    currentLayers |= InteractionLayerMask.GetMask("SphereSelectEdge");
                    break;
                case MeshSelectionUI.SelectionMode.Face:
                    currentLayers |= InteractionLayerMask.GetMask("SphereSelectFace");
                    break;
            }
            
            // Update interaction layers
            directInteractor.interactionLayers = currentLayers;
        }
    }

    public void AdjustRadius(float change)
    {
        currentRadius = Mathf.Clamp(currentRadius + change, minRadius, maxRadius);
        sphereCollider.radius = currentRadius;
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
        if (args.interactableObject.transform != null)
        {
            selectedObjectTransform = args.interactableObject.transform;
            initialPosition = selectedObjectTransform.position;
        }

        // Get collider position in world space
        Vector3 colliderWorldPosition = interactor.transform.TransformPoint(sphereCollider.center);

        var currentMode = meshManipulationLogic.GetCurrentMode();
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
        // Use Physics.OverlapSphere directly to detect objects in range
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
                    
                    if (meshManipulationLogic != null)
                    {
                        meshManipulationLogic.SetSelectedObject(selectInteractable.transform.gameObject);
                    }
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
        if (selectedObjectTransform != null)
        {
            Vector3 finalPosition = selectedObjectTransform.position;
            if (initialPosition != finalPosition)
            {
                var moveCommand = new MoveCommand(selectedObjectTransform, initialPosition, finalPosition);
                UndoRedoManager.Instance.ExecuteCommand(moveCommand);
                Debug.Log("Move command executed");
            }
            selectedObjectTransform = null;
        }

        OnSelectExitedEvent?.Invoke(args);
    }

    public SphereCollider GetSphereCollider()
    {
        return sphereCollider;
    }

    private void OnEnable()
    {
        undoAction.action.Enable();
        redoAction.action.Enable();
        undoAction.action.performed += OnUndo;
        redoAction.action.performed += OnRedo;
    }

    private void OnDisable()
    {
        undoAction.action.performed -= OnUndo;
        redoAction.action.performed -= OnRedo;
        undoAction.action.Disable();
        redoAction.action.Disable();
    }

    private void OnUndo(InputAction.CallbackContext context)
    {
        UndoRedoManager.Instance.Undo();
    }

    private void OnRedo(InputAction.CallbackContext context)
    {
        UndoRedoManager.Instance.Redo();
    }
}