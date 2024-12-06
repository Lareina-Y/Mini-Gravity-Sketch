using UnityEngine;

namespace XRC.Assignments.Project.G09
{
    using UndoRedo.Core;

    namespace UndoRedo.Commands
    {
        public class DeleteObjectCommand : IUndoRedoCommand
        {
            private GameObject deletedObject;
            private Vector3 position;
            private Quaternion rotation;
            private Vector3 scale;

            public string CommandName => "Delete Object";

            public DeleteObjectCommand(GameObject obj)
            {
                this.deletedObject = obj;
                this.position = obj.transform.position;
                this.rotation = obj.transform.rotation;
                this.scale = obj.transform.localScale;
            }

            public DeleteObjectCommand(GameObject obj, Vector3 initialPosition, Quaternion initialRotation)
            {
                this.deletedObject = obj;
                this.position = initialPosition;
                this.rotation = initialRotation;
                this.scale = obj.transform.localScale;
            }

            public void Execute()
            {
                if (deletedObject != null)
                {
                    deletedObject.SetActive(false);
                }
            }

            public void Undo()
            {
                if (deletedObject != null)
                {
                    deletedObject.SetActive(true);
                    deletedObject.transform.position = position;
                    deletedObject.transform.rotation = rotation;
                    deletedObject.transform.localScale = scale;
                }
            }
        }
    }
}