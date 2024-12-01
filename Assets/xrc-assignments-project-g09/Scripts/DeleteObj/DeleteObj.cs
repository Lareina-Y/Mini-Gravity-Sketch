using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

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
    /// This method destroys the object being grabbed by the interactor.
    /// </summary>
    /// <param name="obj"></param>
    private void Delete(InputAction.CallbackContext obj)
    {
        if (!m_Interactor.hasSelection)
        {
            return;
        }

        GameObject selectedObject = m_Interactor.firstInteractableSelected.transform.gameObject;
        Destroy(selectedObject);
    }
}