using UnityEngine;
using UnityEngine.InputSystem;

public class GrabMoveInput : MonoBehaviour
{
    // TODO: Double check input action property setting: one binding / button one binding
    [SerializeField] InputActionProperty m_GrabMoveAction;
    [SerializeField] InputActionProperty m_GrabMoveResetAction;
    
    void OnEnable()
    {
        m_GrabMoveAction.action.Enable();
        m_GrabMoveResetAction.action.Enable();
    }

    void OnDisable()
    {
        m_GrabMoveAction.action.Disable();
        m_GrabMoveResetAction.action.Disable();
    }

    void Start()
    {
        m_GrabMoveAction.action.performed += StartGrabMove;
        m_GrabMoveAction.action.canceled += StopGrabMove;
        m_GrabMoveResetAction.action.performed += ResetGrabMove;
    }

    private void StartGrabMove(InputAction.CallbackContext context)
    {
        GetComponent<GrabMoveLogic>().isGrabMoving = true;
    }

    private void StopGrabMove(InputAction.CallbackContext context)
    {
        GetComponent<GrabMoveLogic>().isGrabMoving = false;
    }

    private void ResetGrabMove(InputAction.CallbackContext context)
    {
        // TODO: Find out what to reset
        Debug.Log("ResetGrabMove");
    }
}
