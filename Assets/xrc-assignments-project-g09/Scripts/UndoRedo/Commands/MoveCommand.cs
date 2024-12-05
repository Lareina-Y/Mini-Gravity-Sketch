using UnityEngine;
using UndoRedo.Core;

namespace UndoRedo.Commands
{
    public class MoveCommand : IUndoRedoCommand
    {
        private Transform targetTransform;
        private Vector3 previousPosition;
        private Vector3 newPosition;

        public string CommandName => "Move Object";

        public MoveCommand(Transform target, Vector3 previousPos, Vector3 newPos)
        {
            targetTransform = target;
            previousPosition = previousPos;
            newPosition = newPos;
        }

        public void Execute()
        {
            targetTransform.position = newPosition;
        }

        public void Undo()
        {
            targetTransform.position = previousPosition;
        }
    }
} 