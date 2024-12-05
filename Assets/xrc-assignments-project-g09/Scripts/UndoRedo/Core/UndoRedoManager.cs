using UnityEngine;
using System.Collections.Generic;

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

        [SerializeField] private int maxUndoSteps = 50;  // Redo max steps
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