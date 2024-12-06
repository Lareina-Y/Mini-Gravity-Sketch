namespace XRC.Assignments.Project.G09
{
    namespace UndoRedo.Core
    {
        public interface IUndoRedoCommand
        {
            string CommandName { get; }
            void Execute();
            void Undo();
        }
    }
}