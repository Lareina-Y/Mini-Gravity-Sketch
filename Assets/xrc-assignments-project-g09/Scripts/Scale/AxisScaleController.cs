using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AxisScaleController : MonoBehaviour
{
    private Transform xAxis;
    private Transform yAxis;
    private Transform zAxis;
    private Vector3 initialGrabPosition;
    private Vector3 initialScale;
    private Transform selectedAxis;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor currentInteractor;
    private float scaleSpeed = 1f;

    private static bool isInScaleMode = false;
    public static bool IsInScaleMode
    {
        get { return isInScaleMode; }
        set { isInScaleMode = value; }
    }

    void Start()
    {
        xAxis = transform.Find("Axis/X-Axis");
        yAxis = transform.Find("Axis/Y-Axis");
        zAxis = transform.Find("Axis/Z-Axis");

        SetupAxisInteraction(xAxis);
        SetupAxisInteraction(yAxis);
        SetupAxisInteraction(zAxis);

        UpdateAxisPositions();
    }

    void SetupAxisInteraction(Transform axis)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = axis.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = false;
        grabInteractable.throwOnDetach = false;

        grabInteractable.selectEntered.AddListener((selectEnterEventArgs) => OnAxisGrabbed(selectEnterEventArgs, axis));
        grabInteractable.selectExited.AddListener((selectExitEventArgs) => OnAxisReleased(selectExitEventArgs, axis));
    }

    void OnAxisGrabbed(SelectEnterEventArgs args, Transform axis)
    {
   
        if (!isInScaleMode) return;

        selectedAxis = axis;
        currentInteractor = args.interactorObject;
        initialGrabPosition = args.interactorObject.transform.position;
        initialScale = transform.localScale;
    }

    void OnAxisReleased(SelectExitEventArgs args, Transform axis)
    {
        if (!isInScaleMode) return;

        selectedAxis = null;
        currentInteractor = null;
    }

    void Update()
    {
        if (isInScaleMode && selectedAxis != null && currentInteractor != null)
        {
            UpdateScale();
        }
    }

    void UpdateScale()
    {
        if (currentInteractor == null) return;

        Vector3 currentPosition = currentInteractor.transform.position;

        float dragDistance = Vector3.Distance(currentPosition, initialGrabPosition);
        float direction = Vector3.Dot((currentPosition - initialGrabPosition).normalized,
                                    selectedAxis.forward) > 0 ? 1 : -1;
        float scaleDelta = dragDistance * direction * scaleSpeed;

        Vector3 newScale = initialScale;
        if (selectedAxis == xAxis)
        {
            newScale.x = Mathf.Max(0.1f, initialScale.x + scaleDelta);
        }
        else if (selectedAxis == yAxis)
        {
            newScale.y = Mathf.Max(0.1f, initialScale.y + scaleDelta);
        }
        else if (selectedAxis == zAxis)
        {
            newScale.z = Mathf.Max(0.1f, initialScale.z + scaleDelta);
        }

        transform.localScale = newScale;
        UpdateAxisPositions();
    }

    void UpdateAxisPositions()
    {
        Vector3 scale = transform.localScale;

        if (xAxis) xAxis.localPosition = new Vector3(scale.x / 2, 0, 0);
        if (yAxis) yAxis.localPosition = new Vector3(0, scale.y / 2, 0);
        if (zAxis) zAxis.localPosition = new Vector3(0, 0, scale.z / 2);
    }
}