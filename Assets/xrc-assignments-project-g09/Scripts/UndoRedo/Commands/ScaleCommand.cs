using UnityEngine;
using UndoRedo.Core;

namespace UndoRedo.Commands
{
    public class ScaleCommand : IUndoRedoCommand
    {
        private Transform targetTransform;
        private Vector3 initialScale;
        private Vector3 finalScale;

        public string CommandName => "Scale Object";

        public ScaleCommand(Transform transform, Vector3 initialScale, Vector3 finalScale)
        {
            this.targetTransform = transform;
            this.initialScale = initialScale;
            this.finalScale = finalScale;
        }

        public void Execute()
        {
            if (targetTransform != null)
            {
                targetTransform.localScale = finalScale;
            }
        }

        public void Undo()
        {
            if (targetTransform != null)
            {
                targetTransform.localScale = initialScale;
            }
        }
    }
} 