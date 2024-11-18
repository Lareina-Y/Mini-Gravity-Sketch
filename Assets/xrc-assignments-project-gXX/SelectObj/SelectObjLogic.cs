using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;

public class SelectObjLogic : MonoBehaviour
{
    [SerializeField]
    public XRBaseInteractor interactor;
    public float minRadius = 0.01f;
    public float maxRadius = 0.3f;
    public float defaultRadius = 0.04f;

    // Transform indicating the default position for the sphere center,
    // set to the right controller grip button.
    public Transform defaultCenter;

    private float currentRadius;
    private List<Collider> selectedObjects = new List<Collider>();

    private void Awake()
    {
        currentRadius = defaultRadius;
    }

    private void Update()
    {
        // Continuously update the sphere's position and radius
        UpdateSphere();
        PerformSelection();
    }

    private void UpdateSphere()
    {
        // Set the radius with clamping between min and max
        currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);
    }

    private void PerformSelection()
    {
        // Use Physics.OverlapSphere to detect objects within the selection sphere
        Vector3 sphereCenter = defaultCenter.position;

        if (currentRadius < defaultRadius)
        {
            sphereCenter.x += currentRadius - defaultRadius;
        }

        Collider[] colliders = Physics.OverlapSphere(sphereCenter, currentRadius);
        selectedObjects.Clear();

        foreach (var collider in colliders)
        {
            if (!selectedObjects.Contains(collider))
            {
                selectedObjects.Add(collider);
            }
        }
    }
    
    public void AdjustRadius(float radiusChange)
    {
        // Update the current radius based on the input value
        currentRadius += radiusChange;
        currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);
    }

    public float getCurrentRadius()
    {
        return currentRadius;
    }
}
