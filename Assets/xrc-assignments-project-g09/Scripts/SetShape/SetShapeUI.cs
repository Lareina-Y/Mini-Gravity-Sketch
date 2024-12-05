using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace SetShape
{
    public class SetShapeUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas selectionCanvas;
        [SerializeField] private RectTransform firstRowPanel;
        [SerializeField] private RectTransform secondRowPanel;
        
        [Header("Visual Feedback")]
        [SerializeField] private Color buttonSelectedColor = new Color(0.3f, 0.7f, 1f, 1f);
        [SerializeField] private Color buttonHoverColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);
        [SerializeField] private float colorTransitionDuration = 0.15f;

        private bool isMenuVisible = false;
        private SetShapeLogic.ShapeType? hoveredShape = null;
        private SetShapeLogic setShapeLogic;
        private TrackedDeviceGraphicRaycaster graphicRaycaster;

        private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        private Dictionary<SetShapeLogic.ShapeType, Coroutine> colorTransitionCoroutines = 
            new Dictionary<SetShapeLogic.ShapeType, Coroutine>();

        private void Start()
        {

            graphicRaycaster = selectionCanvas.gameObject.GetComponent<TrackedDeviceGraphicRaycaster>();
            if (graphicRaycaster == null)
            {
                graphicRaycaster = selectionCanvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            }
            
            setShapeLogic = GetComponent<SetShapeLogic>();
            if (setShapeLogic != null)
            {
                setShapeLogic.OnShapeChanged += OnShapeChanged;
            }

            SetupShapeButtons();
            HideMenu();
        }

        private void SetupShapeButtons()
        {
            foreach (SetShapeLogic.ShapeType shapeType in System.Enum.GetValues(typeof(SetShapeLogic.ShapeType)))
            {
                string buttonName = $"{shapeType} Button";
                
                Transform buttonTransform = firstRowPanel.Find(buttonName);
                if (buttonTransform == null)
                {
                    buttonTransform = secondRowPanel.Find(buttonName);
                }

                if (buttonTransform != null && buttonTransform.TryGetComponent<Button>(out var button))
                {
                    SetupButton(button, shapeType);
                }
                else
                {
                    Debug.LogWarning($"Button for shape {shapeType} not found!");
                }
            }
        }

        private void SetupButton(Button button, SetShapeLogic.ShapeType shapeType)
        {
            var eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();

            // Pointer Enter
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => {
                hoveredShape = shapeType;
                if (setShapeLogic.CurrentShapeType != shapeType)
                {
                    var buttonBackground = button.GetComponent<Image>();
                    if (buttonBackground != null)
                    {
                        TransitionButtonColor(buttonBackground, buttonHoverColor, shapeType);
                    }
                }
            });
            eventTrigger.triggers.Add(entry);

            // Pointer Exit
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener((data) => {
                if (hoveredShape == shapeType)
                    hoveredShape = null;
                if (setShapeLogic.CurrentShapeType != shapeType)
                {
                    var buttonBackground = button.GetComponent<Image>();
                    if (buttonBackground != null)
                    {
                        Color fadeOutColor = buttonBackground.color;
                        fadeOutColor.a = 0f;
                        TransitionButtonColor(buttonBackground, fadeOutColor, shapeType);
                    }
                }
            });
            eventTrigger.triggers.Add(entry);
        }

        private void UpdateButtonVisuals(bool useTransition = true)
        {
            foreach (SetShapeLogic.ShapeType shapeType in System.Enum.GetValues(typeof(SetShapeLogic.ShapeType)))
            {
                string buttonName = $"{shapeType} Button";
                
                Transform buttonTransform = firstRowPanel.Find(buttonName);
                if (buttonTransform == null)
                {
                    buttonTransform = secondRowPanel.Find(buttonName);
                }
                
                if (buttonTransform != null && buttonTransform.TryGetComponent<Button>(out var button))
                {
                    UpdateButtonState(button, shapeType, useTransition);
                }
            }
        }

        private void UpdateButtonState(Button button, SetShapeLogic.ShapeType shapeType, bool useTransition)
        {
            var buttonBackground = button.GetComponent<Image>();
            if (buttonBackground != null)
            {
                if (setShapeLogic.CurrentShapeType == shapeType)
                {
                    if (useTransition)
                    {
                        TransitionButtonColor(buttonBackground, buttonSelectedColor, shapeType);
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
                        TransitionButtonColor(buttonBackground, fadeOutColor, shapeType);
                    }
                    else
                    {
                        buttonBackground.color = fadeOutColor;
                    }
                }
            }
        }

        private void TransitionButtonColor(Image buttonImage, Color targetColor, SetShapeLogic.ShapeType shapeType)
        {
            if (colorTransitionCoroutines.TryGetValue(shapeType, out var existingCoroutine))
            {
                if (existingCoroutine != null)
                    StopCoroutine(existingCoroutine);
            }

            colorTransitionCoroutines[shapeType] = StartCoroutine(AlphaTransitionCoroutine(buttonImage, targetColor));
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

        public void ShowMenu(Vector3 worldPosition, Quaternion rotation)
        {
            if (selectionCanvas == null || firstRowPanel == null || secondRowPanel == null) return;

            selectionCanvas.gameObject.SetActive(true);
            isMenuVisible = true;

            selectionCanvas.transform.position = worldPosition;
            
            // Make menu face the camera
            rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
            selectionCanvas.transform.rotation = rotation;

            UpdateButtonVisuals(false);
        }

        public void HideMenu()
        {
            if (hoveredShape.HasValue)
            {
                setShapeLogic.SetCurrentShape(hoveredShape.Value);
            }

            if (selectionCanvas != null)
            {
                selectionCanvas.gameObject.SetActive(false);
            }
            isMenuVisible = false;
            hoveredShape = null;
        }

        public SetShapeLogic.ShapeType? GetHoveredShape()
        {
            return hoveredShape;
        }

        private void OnShapeChanged(SetShapeLogic.ShapeType newShape)
        {
            UpdateButtonVisuals();
        }

        private void OnDestroy()
        {
            if (setShapeLogic != null)
            {
                setShapeLogic.OnShapeChanged -= OnShapeChanged;
            }
        }
    }
}
