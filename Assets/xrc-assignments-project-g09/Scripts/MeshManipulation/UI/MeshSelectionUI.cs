using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace XRC.Assignments.Project.G09
{
    namespace MeshManipulation.UI
    {
        public class MeshSelectionUI : MonoBehaviour
        {
            public enum SelectionMode
            {
                Object,
                Vertex,
                Edge,
                Face
            }

            [Header("UI References")] [SerializeField]
            private Canvas selectionCanvas;

            [SerializeField] private RectTransform menuPanel;
            [SerializeField] private TrackedDeviceGraphicRaycaster graphicRaycaster;
            [SerializeField] private Image menuBackground;
            [SerializeField] private MeshManipulationLogic meshManipulationLogic;

            [Header("Mode Buttons")] [SerializeField]
            private Button objectModeButton;

            [SerializeField] private Button vertexModeButton;
            [SerializeField] private Button edgeModeButton;
            [SerializeField] private Button faceModeButton;

            [Header("Visual Feedback")] [SerializeField]
            private Color menuBackgroundColor = new Color(1f, 1f, 1f, 0.8f);

            [SerializeField] private Color buttonSelectedColor = new Color(0.3f, 0.7f, 1f, 1f);
            [SerializeField] private Color buttonHoverColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);
            [SerializeField] private float colorTransitionDuration = 0.15f;

            private SelectionMode currentMode = SelectionMode.Object;
            private bool isMenuVisible = false;
            private SelectionMode? hoveredMode = null;

            private readonly Color clearColor = Color.clear;
            private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
            private Coroutine[] colorTransitionCoroutines;

            public delegate void ModeChangedEventHandler(SelectionMode newMode);

            public event ModeChangedEventHandler OnModeChanged;

            public SelectionMode CurrentMode => currentMode;
            public bool IsMenuVisible => isMenuVisible;

            private void Start()
            {
                if (graphicRaycaster == null)
                {
                    graphicRaycaster = selectionCanvas.gameObject.GetComponent<TrackedDeviceGraphicRaycaster>();
                    if (graphicRaycaster == null)
                    {
                        graphicRaycaster = selectionCanvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
                    }
                }

                if (meshManipulationLogic == null)
                {
                    meshManipulationLogic = FindFirstObjectByType<MeshManipulationLogic>();
                }

                colorTransitionCoroutines = new Coroutine[4];

                SetupMenuBackground();

                SetupButton(objectModeButton, SelectionMode.Object);
                SetupButton(vertexModeButton, SelectionMode.Vertex);
                SetupButton(edgeModeButton, SelectionMode.Edge);
                SetupButton(faceModeButton, SelectionMode.Face);

                UpdateButtonVisuals();
                HideMenu();
            }

            private void SetupMenuBackground()
            {
                if (menuBackground != null)
                {
                    menuBackground.color = menuBackgroundColor;
                }
            }

            private void SetupButton(Button button, SelectionMode mode)
            {
                button.onClick.AddListener(() =>
                {
                    currentMode = mode;
                    OnModeChanged?.Invoke(mode);
                    UpdateButtonVisuals();
                    HideMenu();
                });

                var eventTrigger = button.gameObject.GetComponent<EventTrigger>();
                if (eventTrigger == null)
                    eventTrigger = button.gameObject.AddComponent<EventTrigger>();

                // Pointer Enter
                var entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) =>
                {
                    hoveredMode = mode;
                    if (currentMode != mode)
                    {
                        var buttonBackground = button.GetComponent<Image>();
                        if (buttonBackground != null)
                        {
                            TransitionButtonColor(buttonBackground, buttonHoverColor, (int)mode);
                        }
                    }
                });
                eventTrigger.triggers.Add(entry);

                // Pointer Exit
                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerExit;
                entry.callback.AddListener((data) =>
                {
                    if (hoveredMode == mode)
                        hoveredMode = null;
                    if (currentMode != mode)
                    {
                        var buttonBackground = button.GetComponent<Image>();
                        if (buttonBackground != null)
                        {
                            Color fadeOutColor = buttonBackground.color;
                            fadeOutColor.a = 0f;
                            TransitionButtonColor(buttonBackground, fadeOutColor, (int)mode);
                        }
                    }
                });
                eventTrigger.triggers.Add(entry);
            }

            private void UpdateButtonVisuals(bool useTransition = true)
            {
                UpdateButtonState(objectModeButton, SelectionMode.Object, useTransition);
                UpdateButtonState(vertexModeButton, SelectionMode.Vertex, useTransition);
                UpdateButtonState(edgeModeButton, SelectionMode.Edge, useTransition);
                UpdateButtonState(faceModeButton, SelectionMode.Face, useTransition);
            }

            private void UpdateButtonState(Button button, SelectionMode mode, bool useTransition)
            {
                var buttonBackground = button.GetComponent<Image>();
                if (buttonBackground != null)
                {
                    if (currentMode == mode)
                    {
                        if (useTransition)
                        {
                            TransitionButtonColor(buttonBackground, buttonSelectedColor, (int)mode);
                        }
                        else
                        {
                            buttonBackground.color = buttonSelectedColor;
                        }
                    }
                    else
                    {
                        Color fadeOutColor = buttonBackground.color;
                        fadeOutColor.a = 0f;
                        if (useTransition)
                        {
                            TransitionButtonColor(buttonBackground, fadeOutColor, (int)mode);
                        }
                        else
                        {
                            buttonBackground.color = fadeOutColor;
                        }
                    }
                }
            }

            private void TransitionButtonColor(Image buttonImage, Color targetColor, int buttonIndex)
            {

                if (colorTransitionCoroutines[buttonIndex] != null)
                {
                    StopCoroutine(colorTransitionCoroutines[buttonIndex]);
                    colorTransitionCoroutines[buttonIndex] = null;
                }


                colorTransitionCoroutines[buttonIndex] =
                    StartCoroutine(AlphaTransitionCoroutine(buttonImage, targetColor));
            }

            private IEnumerator AlphaTransitionCoroutine(Image image, Color targetColor)
            {
                float startAlpha = image.color.a;
                float targetAlpha = targetColor.a;
                float elapsedTime = 0f;

                Color currentColor = targetColor;
                currentColor.a = startAlpha;
                image.color = currentColor;

                while (elapsedTime < colorTransitionDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / colorTransitionDuration;

                    t = Mathf.SmoothStep(0, 1, t);
                    currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                    image.color = currentColor;

                    yield return waitForEndOfFrame;
                }

                image.color = targetColor;
            }

            public bool TryGetHoveredMode(out SelectionMode mode)
            {
                if (hoveredMode.HasValue)
                {
                    mode = hoveredMode.Value;
                    return true;
                }

                mode = SelectionMode.Object;
                return false;
            }

            public void ShowMenu(Vector3 worldPosition, Quaternion rotation)
            {
                if (selectionCanvas == null || menuPanel == null) return;

                if (meshManipulationLogic != null)
                {
                    currentMode = meshManipulationLogic.GetCurrentMode();
                }

                selectionCanvas.gameObject.SetActive(true);
                isMenuVisible = true;

                selectionCanvas.transform.position = worldPosition;

                // Set z-axis rotation to 0
                rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
                selectionCanvas.transform.rotation = rotation;

                menuPanel.localPosition = Vector3.zero;

                UpdateButtonVisuals(false);
            }

            public void HideMenu()
            {
                if (selectionCanvas != null)
                {
                    selectionCanvas.gameObject.SetActive(false);
                }

                isMenuVisible = false;
                hoveredMode = null;
            }
        }
    }
}