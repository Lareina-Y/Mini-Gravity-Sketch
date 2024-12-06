using UnityEngine;
using System.Collections.Generic; 
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Linq;

namespace XRC.Assignments.Project.G09
{
    using UndoRedo.Core;
    using UndoRedo.Commands;
    public class DeleteObj : MonoBehaviour
    {

        [SerializeField] private XRBaseInteractor m_Interactor;

        [SerializeField] private Transform m_RightTransform;

        [SerializeField] private InputActionProperty m_Delete;

        protected void OnEnable()
        {
            m_Delete.action.Enable();
        }

        protected void OnDisable()
        {
            m_Delete.action.Disable();
        }

        void Start()
        {
            m_Delete.action.performed += Delete;
        }

        /// <summary>
        /// This method destroys all selected objects being grabbed by the interactor.
        /// </summary>
        /// <param name="obj"></param>
        private void Delete(InputAction.CallbackContext obj)
        {
            if (!m_Interactor.hasSelection)
            {
                return;
            }

            List<XRBaseInteractable> interactablesToDelete = m_Interactor.interactablesSelected
                .OfType<XRBaseInteractable>()
                .ToList();

            foreach (var interactable in interactablesToDelete)
            {
                GameObject selectedObject = interactable.transform.gameObject;

                // Get initial transform
                var sphereSelect = FindFirstObjectByType<SphereSelectLogic>();
                var initialTransform = sphereSelect.GetInitialTransform(selectedObject);

                // If initial transform is found, use it to create delete command
                if (initialTransform.HasValue)
                {
                    var deleteCommand = new DeleteObjectCommand(
                        selectedObject,
                        initialTransform.Value.position,
                        initialTransform.Value.rotation);
                    UndoRedoManager.Instance.ExecuteCommand(deleteCommand);
                }
                else
                {
                    // If initial transform is not found, use current transform
                    var deleteCommand = new DeleteObjectCommand(selectedObject);
                    UndoRedoManager.Instance.ExecuteCommand(deleteCommand);
                }
            }
        }
    }
}