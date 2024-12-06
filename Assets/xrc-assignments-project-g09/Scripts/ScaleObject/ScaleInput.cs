using UnityEngine;
using UnityEngine.InputSystem;

public class ScaleInput : MonoBehaviour
{
    [SerializeField] private InputActionProperty m_ChangeScale;

    private ScaleLogic m_ScaleLogic;
    
    void Awake()
    {
        m_ScaleLogic = GetComponent<ScaleLogic>();
        m_ChangeScale.action.performed += OnStartScale;
    }

    void OnEnable()
    {
        m_ChangeScale.action.Enable();
    }

    void OnDisable()
    {
        m_ChangeScale.action.Disable();
    }

    private void OnStartScale(InputAction.CallbackContext context)
    {
        if (m_ScaleLogic.HasGameObject)
        {
            if (m_ScaleLogic.IsScaleActive)
            {
                m_ScaleLogic.EndScale();
            }
            else
            {
                m_ScaleLogic.StartScale();
            }
        }
    }
}
