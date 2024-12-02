using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Change color is triggered by pressing Primary when selecting an item, exit by release the primary button.
/// TODO: trigger logic
/// </summary>
public class ChangeColorInput : MonoBehaviour
{
    [SerializeField] private InputActionProperty m_ChangeColor;

    private ChangeColorLogic m_ChangeColorLogic;
    
    void Awake()
    {
        m_ChangeColorLogic = GetComponent<ChangeColorLogic>();
        m_ChangeColor.action.performed += OnStartColorChange;
        m_ChangeColor.action.canceled += OnEndColorChange;
    }

    void OnEnable()
    {
        m_ChangeColor.action.Enable();
    }

    void OnDisable()
    {
        m_ChangeColor.action.Disable();
    }

    private void OnStartColorChange(InputAction.CallbackContext context)
    {
        m_ChangeColorLogic.ChangeColor();
    }

    private void OnEndColorChange(InputAction.CallbackContext context)
    {
        m_ChangeColorLogic.EndChangeColor();
    }
}
