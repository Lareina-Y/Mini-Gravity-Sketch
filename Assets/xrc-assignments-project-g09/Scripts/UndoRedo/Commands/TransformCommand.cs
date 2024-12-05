using UnityEngine;
using UndoRedo.Core;

namespace UndoRedo.Commands
{
    public class TransformCommand : IUndoRedoCommand
    {
        private Transform targetTransform;
        private Vector3 initialPosition;
        private Vector3 currentPosition;
        private Quaternion initialRotation;
        private Quaternion currentRotation;

        public string CommandName => "Transform Change";

        public TransformCommand(Transform target, Vector3 initialPos, Vector3 currentPos, Quaternion initialRot, Quaternion currentRot)
        {
            targetTransform = target;
            initialPosition = initialPos;
            currentPosition = currentPos;
            initialRotation = initialRot;
            currentRotation = currentRot;
        }

        public void Execute()
        {
            targetTransform.position = initialPosition;
            targetTransform.rotation = initialRotation;
        }

        public void Undo()
        {
            targetTransform.position = currentPosition;
            targetTransform.rotation = currentRotation;
        }
    }
}