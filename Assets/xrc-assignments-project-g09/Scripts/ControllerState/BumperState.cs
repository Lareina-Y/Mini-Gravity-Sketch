using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G09
{
    public class BumperState : MonoBehaviour
    {
        [SerializeField] private InputActionProperty m_LeftGripAction;

        [SerializeField] private InputActionProperty m_RightGripAction;

        [SerializeField] private Renderer leftBumperRenderer; // Renderer of the left bumper

        [SerializeField] private Renderer rightBumperRenderer; // Renderer of the left bumper


        [SerializeField] private Color pressedColor = Color.red; // Color when bumper is pressed

        [SerializeField] private Color defaultColor = Color.grey; // Default color

        // Enable the input action
        protected void OnEnable()
        {
            m_LeftGripAction.action.Enable();
            m_RightGripAction.action.Enable();
        }

        // Disable the input action
        protected void OnDisable()
        {
            m_LeftGripAction.action.Disable();
            m_RightGripAction.action.Disable();
        }

        // Initialize the default color and set up the input event
        void Start()
        {
            leftBumperRenderer.material.color = defaultColor;
            rightBumperRenderer.material.color = defaultColor;

            // Listen for grip action input
            m_LeftGripAction.action.performed += OnLeftGripPerformed;
            m_LeftGripAction.action.canceled += OnLeftGripCanceled;
            m_RightGripAction.action.performed += OnRightGripPerformed;
            m_RightGripAction.action.canceled += OnRightGripCanceled;
        }

        // Change color when the left grip is performed
        private void OnLeftGripPerformed(InputAction.CallbackContext context)
        {
            leftBumperRenderer.material.color = pressedColor;
        }

        // Revert color when the left grip is released
        private void OnLeftGripCanceled(InputAction.CallbackContext context)
        {
            leftBumperRenderer.material.color = defaultColor;
        }

        // Change color when the right grip is performed
        private void OnRightGripPerformed(InputAction.CallbackContext context)
        {
            rightBumperRenderer.material.color = pressedColor;
        }

        // Revert color when the right grip is released
        private void OnRightGripCanceled(InputAction.CallbackContext context)
        {
            rightBumperRenderer.material.color = defaultColor;
        }

    }
}