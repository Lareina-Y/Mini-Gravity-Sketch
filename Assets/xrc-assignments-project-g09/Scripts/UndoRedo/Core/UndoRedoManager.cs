using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace UndoRedo.Core
{
    public class UndoRedoManager : MonoBehaviour
    {
        // Singleton Pattern
        private static UndoRedoManager instance;
        public static UndoRedoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<UndoRedoManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("UndoRedoManager");
                        instance = go.AddComponent<UndoRedoManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        [SerializeField] private int maxUndoSteps = 50;  // Maximum number of undo steps
        [Header("Input Settings")]
        [SerializeField] private InputActionProperty undoRedoAction;
        private bool canTriggerUndoRedo = true;
        private float undoRedoThreshold = 0.8f; // Threshold for joystick input to trigger undo/redo

        private Stack<IUndoRedoCommand> undoStack = new Stack<IUndoRedoCommand>();
        private Stack<IUndoRedoCommand> redoStack = new Stack<IUndoRedoCommand>();

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            undoRedoAction.action.Enable();
            undoRedoAction.action.performed += OnUndoRedoAction;
            undoRedoAction.action.canceled += OnUndoRedoReleased;
        }

        private void OnDisable()
        {
            undoRedoAction.action.performed -= OnUndoRedoAction;
            undoRedoAction.action.canceled -= OnUndoRedoReleased;
            undoRedoAction.action.Disable();
        }

        private void OnUndoRedoAction(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            float inputX = input.x;

            // Only trigger when allowed and threshold is reached
            if (canTriggerUndoRedo)
            {
                if (inputX <= -undoRedoThreshold && CanUndo)
                {
                    Undo();
                    canTriggerUndoRedo = false;
                }
                else if (inputX >= undoRedoThreshold && CanRedo)
                {
                    Redo();
                    canTriggerUndoRedo = false;
                }
            }
        }

        private void OnUndoRedoReleased(InputAction.CallbackContext context)
        {
            canTriggerUndoRedo = true; // Allow new triggers when joystick returns to center
        }

        public void ExecuteCommand(IUndoRedoCommand command)
        {
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear();

            if (undoStack.Count > maxUndoSteps)
            {
                var tempStack = new Stack<IUndoRedoCommand>();
                for (int i = 0; i < maxUndoSteps; i++)
                {
                    tempStack.Push(undoStack.Pop());
                }
                undoStack = new Stack<IUndoRedoCommand>(tempStack);
            }
        }

        public void Undo()
        {
            if (CanUndo)
            {
                var command = undoStack.Pop();
                command.Undo();
                redoStack.Push(command);
            }
            else
            {
                Debug.Log("Undo: No command to undo");
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                var command = redoStack.Pop();
                command.Execute();
                undoStack.Push(command);
            }
            else
            {
                Debug.Log("Redo: No command to redo");
            }
        }

        public void ClearHistory()
        {
            undoStack.Clear();
            redoStack.Clear();
        }
    }
}