using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G09
{
    
    using MeshManipulation.UI;
    
    namespace MeshManipulation
    {
        public class MeshManipulationLogic : MonoBehaviour
        {
            [SerializeField] private MeshSelectionUI selectionUI;
            [SerializeField] private MeshEditConfig meshEditConfig;

            private MeshSelectionUI.SelectionMode currentMode;
            private bool isInSelectionMode = false;

            // Store selected objects
            private List<GameObject> selectedObjects = new List<GameObject>();

            // Events for object selection state changes
            public event System.Action<GameObject> OnObjectSelectionChanged;

            // Event for mode changes
            public event System.Action<MeshSelectionUI.SelectionMode> OnModeChanged;

            // Get currently selected objects (returns array for future expansion)
            public GameObject[] SelectedObjects => selectedObjects.ToArray();

            public bool HasSelectedObject => selectedObjects.Count > 0;

            private void Start()
            {
                currentMode = MeshSelectionUI.SelectionMode.Object;
                if (selectionUI != null)
                {
                    selectionUI.OnModeChanged += HandleModeSelection;
                }
            }

            private void OnDestroy()
            {
                if (selectionUI != null)
                {
                    selectionUI.OnModeChanged -= HandleModeSelection;
                }
            }

            public void HandleModeSelection(MeshSelectionUI.SelectionMode newMode)
            {
                if (currentMode == newMode) return;

                currentMode = newMode;
                isInSelectionMode = true;

                if (selectedObjects.Count > 0)
                {
                    GameObject selectedObject = selectedObjects[0];

                    // Set interaction layers based on mode
                    if (selectedObject
                        .TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>(
                            out var interactable))
                    {
                        switch (newMode)
                        {
                            case MeshSelectionUI.SelectionMode.Object:
                                interactable.interactionLayers = InteractionLayerMask.GetMask("SphereSelectObject");
                                // If MeshEditController exists, call ExitEditMode
                                if (selectedObject.TryGetComponent<MeshEditController>(out var editController))
                                {
                                    editController.ExitEditMode();
                                }

                                break;
                            default:
                                interactable.interactionLayers = InteractionLayerMask.GetMask("None");
                                break;
                        }
                    }

                    // Get or add MeshEditController
                    var meshEditController = selectedObject.GetComponent<MeshEditController>();
                    if (meshEditController == null)
                    {
                        meshEditController = selectedObject.AddComponent<MeshEditController>();
                        meshEditController.SetConfig(meshEditConfig);
                        meshEditController.Initialize();
                    }

                    if (currentMode != MeshSelectionUI.SelectionMode.Object)
                    {
                        meshEditController.ShowForMode(currentMode);
                    }
                    else
                    {
                        meshEditController.HideAll();
                    }
                }

                OnModeChanged?.Invoke(currentMode);
            }

            private void ClearCurrentSelection()
            {
                if (selectedObjects.Count > 0)
                {
                    selectedObjects.Clear();
                }

                switch (currentMode)
                {
                    case MeshSelectionUI.SelectionMode.Vertex:
                        break;
                    case MeshSelectionUI.SelectionMode.Edge:
                        break;
                    case MeshSelectionUI.SelectionMode.Face:
                        break;
                }
            }

            public MeshSelectionUI.SelectionMode GetCurrentMode()
            {
                return currentMode;
            }

            public bool IsInSelectionMode()
            {
                return isInSelectionMode;
            }

            public void SetSelectedObject(GameObject obj)
            {
                selectedObjects.Clear();
                if (obj != null)
                {
                    selectedObjects.Add(obj);
                    OnObjectSelectionChanged?.Invoke(obj);
                }
            }

            public GameObject GetSelectedObject()
            {
                return selectedObjects.Count > 0 ? selectedObjects[0] : null;
            }

            // Method to clear selected objects
            public void ClearSelection()
            {
                if (selectedObjects.Count > 0)
                {
                    selectedObjects.Clear();
                    OnObjectSelectionChanged?.Invoke(null);
                }
            }

            // Method to check if an object is selected
            public bool IsObjectSelected(GameObject obj)
            {
                return selectedObjects.Contains(obj);
            }
        }
    }
}
