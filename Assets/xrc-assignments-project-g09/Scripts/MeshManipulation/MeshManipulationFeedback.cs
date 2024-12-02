using UnityEngine;
using MeshManipulation.UI;
using System.Collections.Generic;

namespace MeshManipulation
{
    public class MeshManipulationFeedback : MonoBehaviour
    {
        [SerializeField] private MeshSelectionUI selectionUI;
        [SerializeField] private float menuDistance = 0.3f;
        [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;

        [Header("Material Settings")]
        [SerializeField] private Material editModeMaterial;
        
        private Camera mainCamera;
        private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
        private MeshManipulationLogic meshManipulationLogic;

        private void Start()
        {
            mainCamera = Camera.main;
            
            meshManipulationLogic = GetComponent<MeshManipulationLogic>();
            if (meshManipulationLogic != null)
            {
                meshManipulationLogic.OnModeChanged += HandleModeChanged;
            }

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

            if (editModeMaterial == null)
            {
                Debug.LogError("Edit Mode Material is not assigned!");
            }
        }

        private void HandleModeChanged(MeshSelectionUI.SelectionMode mode)
        {
            Debug.Log("Feedback: HandleModeChanged: " + mode);
            bool isEditMode = mode != MeshSelectionUI.SelectionMode.Object;
            UpdateMaterialsForEditMode(isEditMode);
        }

        private void UpdateMaterialsForEditMode(bool isEditMode)
        {
            var selectedObjects = meshManipulationLogic.SelectedObjects;
            foreach (var obj in selectedObjects)
            {
                if (obj != null && obj.TryGetComponent<MeshRenderer>(out var renderer))
                {
                    if (isEditMode)
                    {
                        if (!originalMaterials.ContainsKey(obj))
                        {
                            originalMaterials[obj] = renderer.material;
                            renderer.material = editModeMaterial;
                        }
                    }
                    else
                    {
                        if (originalMaterials.ContainsKey(obj))
                        {
                            renderer.material = originalMaterials[obj];
                            originalMaterials.Remove(obj);
                        }
                    }
                }
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

        private void OnDestroy()
        {
            if (meshManipulationLogic != null)
            {
                meshManipulationLogic.OnModeChanged -= HandleModeChanged;
            }

            // Restore all original materials
            foreach (var kvp in originalMaterials)
            {
                if (kvp.Key != null && kvp.Key.TryGetComponent<MeshRenderer>(out var renderer))
                {
                    renderer.material = kvp.Value;
                }
            }
            originalMaterials.Clear();
        }
    }
}
