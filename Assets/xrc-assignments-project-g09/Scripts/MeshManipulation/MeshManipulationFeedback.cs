using UnityEngine;
using MeshManipulation.UI;


namespace MeshManipulation
{
    public class MeshManipulationFeedback : MonoBehaviour
    {
        [SerializeField] private MeshSelectionUI selectionUI;
        [SerializeField] private float menuDistance = 0.3f;
        [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
        
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            
            var input = GetComponent<MeshManipulationInput>();
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

            Vector3 rayOrigin = rayInteractor.transform.position;
            Vector3 rayDirection = rayInteractor.transform.forward;
            
            Vector3 menuPosition = rayOrigin + rayDirection * menuDistance;
            
            Vector3 fromCamera = menuPosition - mainCamera.transform.position;
            Vector3 cameraUp = mainCamera.transform.up;
            
            Quaternion menuRotation = Quaternion.LookRotation(fromCamera, cameraUp);
            
            selectionUI.ShowMenu(menuPosition, menuRotation);
        }

        private void HandleMenuHide(Vector3 position)
        {
            if (selectionUI.TryGetHoveredMode(out MeshSelectionUI.SelectionMode hoveredMode))
            {
                var logic = GetComponent<MeshManipulationLogic>();
                logic?.HandleModeSelection(hoveredMode);
            }
            
            selectionUI.HideMenu();
        }
    }
}
