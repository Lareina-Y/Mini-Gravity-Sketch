using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace SetShape
{
    public class SetShapeFeedback : MonoBehaviour
    {
        [SerializeField] private float menuDistance = 0.3f;
        [SerializeField] private XRRayInteractor rayInteractor;

        private SetShapeUI shapeUI;
        private Camera mainCamera;
        private SetShapeLogic setShapeLogic;

        private void Start()
        {
            mainCamera = Camera.main;
            
            setShapeLogic = GetComponent<SetShapeLogic>();
            shapeUI = GetComponent<SetShapeUI>();

            var input = GetComponent<SetShapeInput>();
            if (input != null)
            {
                input.OnMenuShowRequest += HandleMenuShow;
                input.OnMenuHideRequest += HandleMenuHide;
            }

            if (rayInteractor == null)
            {
                Debug.LogError("RayInteractor is not assigned!");
            }
        }

        private void HandleMenuShow(Vector3 position)
        {
            if (rayInteractor == null) return;

            // Enable ray
            rayInteractor.enabled = true;
            if (rayInteractor.TryGetComponent<LineRenderer>(out var lineRenderer))
            {
                lineRenderer.enabled = true;
            }

            Vector3 rayOrigin = rayInteractor.transform.position;
            Vector3 rayDirection = rayInteractor.transform.forward;
            Vector3 menuPosition = rayOrigin + rayDirection * menuDistance;
            
            Vector3 fromCamera = menuPosition - mainCamera.transform.position;
            Vector3 cameraUp = mainCamera.transform.up;
            Quaternion menuRotation = Quaternion.LookRotation(fromCamera, cameraUp);
            
            shapeUI.ShowMenu(menuPosition, menuRotation);
        }

        private void HandleMenuHide()
        {
            // Disable ray
            if (rayInteractor != null)
            {
                rayInteractor.enabled = false;
                if (rayInteractor.TryGetComponent<LineRenderer>(out var lineRenderer))
                {
                    lineRenderer.enabled = false;
                }
            }

            shapeUI.HideMenu();
        }

        private void OnDestroy()
        {
            var input = GetComponent<SetShapeInput>();
            if (input != null)
            {
                input.OnMenuShowRequest -= HandleMenuShow;
                input.OnMenuHideRequest -= HandleMenuHide;
            }
        }
    }
} 