using UnityEngine;
using MeshManipulation.UI;

namespace MeshManipulation
{
    public class MeshManipulationLogic : MonoBehaviour
    {
        [SerializeField] private MeshSelectionUI selectionUI;
        
        private MeshSelectionUI.SelectionMode currentMode;
        private GameObject selectedObject;
        private bool isInSelectionMode = false;

        // Add mode changed event
        public event System.Action<MeshSelectionUI.SelectionMode> OnModeChanged;

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

            ClearCurrentSelection();
            
            currentMode = newMode;
            isInSelectionMode = true;
            
            OnModeChanged?.Invoke(currentMode);
            
            switch (currentMode)
            {
                case MeshSelectionUI.SelectionMode.Object:
                    Debug.Log("Current mode: Object");
                    break;
                case MeshSelectionUI.SelectionMode.Vertex:
                    Debug.Log("Current mode: Vertex");
                    break;
                case MeshSelectionUI.SelectionMode.Edge:
                    Debug.Log("Current mode: Edge");
                    break;
                case MeshSelectionUI.SelectionMode.Face:
                    Debug.Log("Current mode: Face");
                    break;
            }
        }

        private void ClearCurrentSelection()
        {
            if (selectedObject != null)
            {
                selectedObject = null;
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
            selectedObject = obj;
        }

        public GameObject GetSelectedObject()
        {
            return selectedObject;
        }
    }
}
