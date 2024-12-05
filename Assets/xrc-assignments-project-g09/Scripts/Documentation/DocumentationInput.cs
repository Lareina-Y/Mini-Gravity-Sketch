using UnityEngine;
using UnityEngine.InputSystem;

public class DocumentationInput : MonoBehaviour
{
    [SerializeField] private InputActionProperty m_Documentation;
    
    
    void Awake()
    {
        m_Documentation.action.performed += OnOpenDocument;
    }

    void OnEnable()
    {
        m_Documentation.action.Enable();
    }

    void OnDisable()
    {
        m_Documentation.action.Disable();
    }

    private void OnOpenDocument(InputAction.CallbackContext context)
    {
        
    }
}
