using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace XRC.Assignments.Project.G09
{
    namespace SetShape
    {
        public class SetShapeFeedback : MonoBehaviour
        {
            [SerializeField] private float menuDistance = 0.3f;

            [SerializeField] private GameObject rayInteractorObject;

            private XRRayInteractor rayInteractor;
            private LineRenderer rayLineRenderer;

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

                if (rayInteractorObject == null)
                {
                    Debug.LogError("RayInteractorObject is not assigned!");
                }
                else
                {
                    rayInteractor = rayInteractorObject.GetComponent<XRRayInteractor>();
                    rayLineRenderer = rayInteractorObject.GetComponent<LineRenderer>();
                }
            }

            private void HandleMenuShow(Vector3 position)
            {
                if (rayInteractor == null) return;

                // Enable ray
                rayInteractorObject.SetActive(true);

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
                rayInteractorObject.SetActive(false);

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
}