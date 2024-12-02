using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace MeshManipulation
{
    public class MeshManipulationInput : MonoBehaviour
    {
        [SerializeField] private InputActionProperty m_showMenuAction;
        [SerializeField] private float holdThreshold = 0.5f;
        
        public delegate void MenuInputEventHandler(Vector3 position);
        public event MenuInputEventHandler OnMenuShowRequest;
        public event MenuInputEventHandler OnMenuHideRequest;

        private bool isButtonPressed = false;
        private float pressStartTime;
        private bool hasTriggeredHold = false;

        private void Start()
        {
            if (m_showMenuAction != null)
            {
                m_showMenuAction.action.started += OnShowMenuInput;
                m_showMenuAction.action.canceled += OnHideMenuInput;
            }
        }

        private void Update()
        {
            if (isButtonPressed && !hasTriggeredHold)
            {
                if (Time.time - pressStartTime >= holdThreshold)
                {
                    hasTriggeredHold = true;
                    Vector3 cameraPosition = Camera.main.transform.position;
                    OnMenuShowRequest?.Invoke(cameraPosition);
                }
            }
        }

        private void OnEnable()
        {
            if (m_showMenuAction != null)
            {
                m_showMenuAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (m_showMenuAction != null)
            {
                m_showMenuAction.action.Disable();
            }
        }

        private void OnDestroy()
        {
            if (m_showMenuAction != null)
            {
                m_showMenuAction.action.started -= OnShowMenuInput;
                m_showMenuAction.action.canceled -= OnHideMenuInput;
            }
        }

        private void OnShowMenuInput(InputAction.CallbackContext context)
        {
            isButtonPressed = true;
            pressStartTime = Time.time;
            hasTriggeredHold = false;
        }

        private void OnHideMenuInput(InputAction.CallbackContext context)
        {
            isButtonPressed = false;
            if (hasTriggeredHold)
            {
                Vector3 cameraPosition = Camera.main.transform.position;
                OnMenuHideRequest?.Invoke(cameraPosition);
            }
            hasTriggeredHold = false;
        }
    }
}