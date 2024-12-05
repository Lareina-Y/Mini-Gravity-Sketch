using UnityEngine;
using System.Collections.Generic; 
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Linq;

public class DeleteObj : MonoBehaviour
{

    [SerializeField]
    private XRBaseInteractor m_Interactor;
    
    [SerializeField]
    private Transform m_RightTransform;
    
    [SerializeField]
    private InputActionProperty m_Delete;

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
        
        // Create a list of interactables cast to XRBaseInteractable
        List<XRBaseInteractable> interactablesToDelete = m_Interactor.interactablesSelected
            .OfType<XRBaseInteractable>()
            .ToList();

        // Iterate through the copied list and destroy each object
        foreach (var interactable in interactablesToDelete)
        {
            GameObject selectedObject = interactable.transform.gameObject;
            Destroy(selectedObject);
        }
        
    }
}