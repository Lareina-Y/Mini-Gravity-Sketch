using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G09
{
    public class DocumentInput : MonoBehaviour
    {
        [SerializeField] private InputActionProperty m_Documentation;
        [SerializeField] private GameObject m_Panel;

        void Awake()
        {
            m_Documentation.action.performed += OnOpenPanel;
        }

        void OnEnable()
        {
            m_Documentation.action.Enable();
        }

        void OnDisable()
        {
            m_Documentation.action.Disable();
        }

        private void OnOpenPanel(InputAction.CallbackContext context)
        {
            if (m_Panel.activeSelf)
            {
                m_Panel.SetActive(false);
            }
            else
            {
                m_Panel.SetActive(true);
            }
        }
    }
}
