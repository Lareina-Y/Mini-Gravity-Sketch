namespace XRC.Assignments.Project.G09
{
    using UnityEngine;
    using UndoRedo.Core;

    namespace UndoRedo.Commands
    {
        public class ChangeColorCommand : IUndoRedoCommand
        {
            private Renderer targetRenderer;
            private Color initialColor;
            private Color newColor;

            public string CommandName => "Change Color";

            public ChangeColorCommand(Renderer renderer, Color initialColor, Color newColor)
            {
                this.targetRenderer = renderer;
                this.initialColor = initialColor;
                this.newColor = newColor;
            }

            public void Execute()
            {
                targetRenderer.material.color = newColor;
            }

            public void Undo()
            {
                targetRenderer.material.color = initialColor;
            }
        }
    }
}