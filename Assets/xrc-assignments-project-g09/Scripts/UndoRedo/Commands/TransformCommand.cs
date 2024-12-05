using UnityEngine;
using UndoRedo.Core;

namespace UndoRedo.Commands
{
    public class TransformCommand : IUndoRedoCommand
    {
        private Transform targetTransform;
        private Vector3 oldPosition;
        private Vector3 newPosition;
        private Quaternion oldRotation;
        private Quaternion newRotation;

        public string CommandName => "Transform Change";

        public TransformCommand(Transform target, Vector3 newPos, Quaternion newRot)
        {
            targetTransform = target;
            oldPosition = target.position;
            oldRotation = target.rotation;
            newPosition = newPos;
            newRotation = newRot;
        }

        public void Execute()
        {
            targetTransform.position = newPosition;
            targetTransform.rotation = newRotation;
        }

        public void Undo()
        {
            targetTransform.position = oldPosition;
            targetTransform.rotation = oldRotation;
        }
    }
}