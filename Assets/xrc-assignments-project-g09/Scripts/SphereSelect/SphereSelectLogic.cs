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
    
    private SphereCollider sphereCollider; 

    void Awake()
    {
        sphereCollider = interactor.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        
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
    
     private void Update()
     {
         setCenter();
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
    private void setCenter()
    {
        Vector3 sphereCenter = interactor.transform.InverseTransformPoint(defaultCenter.position);

        if (sphereCollider.radius < defaultRadius) 
        {
             sphereCenter.x += sphereCollider.radius - defaultRadius;
        }

        sphereCollider.center = sphereCenter;
    }
    
    public void AdjustRadius(float radiusChange)
    {
        float radius = Mathf.Clamp(sphereCollider.radius + radiusChange, minRadius, maxRadius);
        sphereCollider.radius = radius; 
    }
    public float GetCurrentRadius()
    {
         return sphereCollider.radius;
    }
    public SphereCollider GetSphereCollider()
    {
        return sphereCollider;
    }
}