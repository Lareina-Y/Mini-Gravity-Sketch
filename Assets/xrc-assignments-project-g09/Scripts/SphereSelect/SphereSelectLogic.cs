using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SphereSelectLogic : MonoBehaviour
{
    [SerializeField] public XRBaseInteractor interactor;
    [SerializeField] private float minRadius = 0.01f;
    [SerializeField] private float maxRadius = 0.3f;
    [SerializeField] public float defaultRadius = 0.04f;

    // Transform indicating the default position for the sphere center,
    // set to the right controller grip button.
    [SerializeField] public Transform defaultCenter;

    private float currentRadius;
    private SphereCollider sphereCollider; 

    private void Awake()
    {
        currentRadius = defaultRadius;
        
        // Add and configure the SphereCollider
        sphereCollider = gameObject.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        
        sphereCollider.isTrigger = true;
        sphereCollider.radius = defaultRadius;
    }
    
    void OnEnable()
    {
        interactor.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        interactor.selectEntered.RemoveListener(OnSelectEntered);
    }
    
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        MultiSelect();
    }
    
    private void MultiSelect()
    {
        foreach (var interactable in interactor.interactablesHovered)
        {
            if (interactable is IXRSelectInteractable selectInteractable)
            {
                interactor.interactionManager.SelectEnter(interactor, selectInteractable);
            }
        }
    }

    private void Update()
    {
        setCenter();
    }
    
    private void setCenter()
    {
        Vector3 sphereCenter = interactor.transform.InverseTransformPoint(defaultCenter.position);

        if (currentRadius < defaultRadius)
        {
            sphereCenter.x += currentRadius - defaultRadius;
        }

        sphereCollider.center = sphereCenter;
    }
    
    public void AdjustRadius(float radiusChange)
    {
        // Update the current radius based on the input value
        currentRadius += radiusChange;
        currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);
        sphereCollider.radius = currentRadius;
    }

    public float GetCurrentRadius()
    {
        return currentRadius;
    }
    public SphereCollider GetSphereCollider()
    {
        return sphereCollider;
    }
}