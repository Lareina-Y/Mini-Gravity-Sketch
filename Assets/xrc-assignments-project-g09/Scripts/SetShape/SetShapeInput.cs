using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace SetShape
{
    public class SetShapeInput : MonoBehaviour
    {
        [SerializeField] private InputActionProperty m_showMenuAction;
        [SerializeField] private float holdThreshold = 0.5f;
        
        public delegate void MenuShowEventHandler(Vector3 position);
        public delegate void MenuHideEventHandler();

        public event MenuShowEventHandler OnMenuShowRequest;
        public event MenuHideEventHandler OnMenuHideRequest;

        private bool isButtonPressed = false;
        private float pressStartTime;
        private bool hasTriggeredHold = false;
        private SetShapeLogic setShapeLogic;

        private void Start()
        {
            setShapeLogic = GetComponent<SetShapeLogic>();
            
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
                OnMenuHideRequest?.Invoke();
            }
            hasTriggeredHold = false;
        }
    }
}
