using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace XRC.Assignments.Project.G09
{
    using UndoRedo.Core;
    using UndoRedo.Commands;
    
    /// <summary>
    /// Contains the main logic for Color Change.
    /// </summary>
    public class ChangeColorLogic : MonoBehaviour
    {

        public static ChangeColorLogic Instance { get; private set; }

        public Color Color
        {
            get => m_Color;
        }

        [SerializeField] private XRBaseInteractor m_Interactor;
        [SerializeField] private GameObject m_PointerPrefab;
        [SerializeField] private SphereSelectLogic m_SphereSelectLogic;

        private ColorPanel m_ColorPanel;
        private Color m_Color = Color.white;
        private GameObject m_Pointer;
        private Pose m_AttachPose;
        private GameObject m_TargetObject; // TODO: material issue

        private bool m_IsChanging;
        private Color m_InitialColor;

        public XRBaseInteractor interactor => m_Interactor;
        public static bool IsChangingColor { get; private set; }

        void Awake()
        {
            m_ColorPanel = GetComponent<ColorPanel>();
            if (m_SphereSelectLogic == null)
            {
                m_SphereSelectLogic = FindFirstObjectByType<SphereSelectLogic>();
            }

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            m_Interactor.selectEntered.AddListener(OnSelectEntered);
        }

        void OnDisable()
        {
            m_Interactor.selectEntered.RemoveListener(OnSelectEntered);
        }

        void Start()
        {
            // Spawn a pointer object
            m_Pointer = Instantiate(m_PointerPrefab, m_Interactor.transform);
            m_Pointer.transform.localPosition = new Vector3(0, 0, -0.006f);
            m_Pointer.SetActive(false);
        }

        void Update()
        {
            if (m_IsChanging && m_TargetObject != null)
            {
                m_Color = m_ColorPanel.color;
                m_TargetObject.GetComponent<Renderer>().material.color = m_Color;
            }
        }

        private void OnSelectEntered(SelectEnterEventArgs eventArgs)
        {
            if (eventArgs.interactableObject is IXRSelectInteractable selectInteractable)
            {
                m_TargetObject = selectInteractable.transform.gameObject;
                m_InitialColor = m_TargetObject.GetComponent<Renderer>().material.color;
                m_AttachPose = selectInteractable.GetAttachPoseOnSelect(m_Interactor);
            }
        }

        public void ChangeColor()
        {
            if (m_TargetObject != null)
            {
                IsChangingColor = true;

                // Release the object and move to poseOnSelect
                m_Interactor.interactionManager.SelectExit(m_Interactor, m_Interactor.firstInteractableSelected);
                m_ColorPanel.SetActive(true);
                m_Pointer.SetActive(true);
                m_IsChanging = true;

                // Get initial transform from SphereSelectLogic
                var initialTransform = m_SphereSelectLogic.GetInitialTransform(m_TargetObject);
                if (initialTransform.HasValue)
                {
                    Vector3 initialPosition = initialTransform.Value.position;
                    Quaternion initialRotation = initialTransform.Value.rotation;

                    // Move object to initial position and rotation
                    m_TargetObject.transform.position = initialPosition;
                    m_TargetObject.transform.rotation = initialRotation;
                }
            }
        }

        public void EndChangeColor()
        {
            if (m_TargetObject != null)
            {
                Color finalColor = m_TargetObject.GetComponent<Renderer>().material.color;
                if (m_InitialColor != finalColor)
                {
                    var changeColorCommand = new ChangeColorCommand(
                        m_TargetObject.GetComponent<Renderer>(),
                        m_InitialColor,
                        finalColor
                    );
                    UndoRedoManager.Instance.ExecuteCommand(changeColorCommand);
                }
            }

            m_TargetObject = null;
            m_ColorPanel.SetActive(false);
            m_Pointer.SetActive(false);
            m_IsChanging = false;
            IsChangingColor = false;
        }
    }
}
