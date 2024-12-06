namespace XRC.Assignments.Project.G09
{
    using UnityEngine;
    using UndoRedo.Core;

    namespace UndoRedo.Commands
    {
        public class CreateObjectCommand : IUndoRedoCommand
        {
            private GameObject createdObject;
            private Vector3 position;
            private Quaternion rotation;
            private Vector3 scale;
            private GameObject prefab;

            public string CommandName => "Create Object";

            public CreateObjectCommand(GameObject obj, GameObject prefab)
            {
                this.createdObject = obj;
                this.position = obj.transform.position;
                this.rotation = obj.transform.rotation;
                this.scale = obj.transform.localScale;
                this.prefab = prefab;
            }

            public void Execute()
            {
                if (createdObject == null)
                {
                    createdObject = GameObject.Instantiate(prefab, position, rotation);
                    createdObject.transform.localScale = scale;
                }
                else
                {
                    createdObject.SetActive(true);
                }
            }

            public void Undo()
            {
                if (createdObject != null)
                {
                    createdObject.SetActive(false);
                }
            }
        }
    }
}