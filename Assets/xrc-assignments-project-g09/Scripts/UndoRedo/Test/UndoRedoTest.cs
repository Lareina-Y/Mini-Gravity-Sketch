using UnityEngine;
using UndoRedo.Core;
using UndoRedo.Commands;
using UnityEngine.InputSystem;

namespace UndoRedo.Test
{
    public class UndoRedoTest : MonoBehaviour
    {
        [SerializeField] private Transform testObject;
        private Vector3 randomPosition => Random.insideUnitSphere * 5f;
        private Quaternion randomRotation => Quaternion.Euler(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // Press Space to randomly transform
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                var command = new TransformCommand(testObject, randomPosition, randomRotation);
                UndoRedoManager.Instance.ExecuteCommand(command);
            }

            // Press Z to undo
            if (keyboard.zKey.wasPressedThisFrame)
            {
                UndoRedoManager.Instance.Undo();
            }

            // Press Y to redo
            if (keyboard.yKey.wasPressedThisFrame)
            {
                UndoRedoManager.Instance.Redo();
            }
        }
    }
}