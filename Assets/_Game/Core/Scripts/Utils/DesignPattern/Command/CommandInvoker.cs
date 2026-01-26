using System.Collections.Generic;

namespace _Game.Core.Scripts.Utils.DesignPattern.Command
{
    public class CommandInvoker
    {
        private Stack<ICommand> _undoStack = new Stack<ICommand>();
        private Stack<ICommand> _redoStack = new Stack<ICommand>();

        public void Excute(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                ICommand activeCommand = _undoStack.Pop();
                activeCommand.Undo();
                _redoStack.Push(activeCommand);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                ICommand activeCommand = _redoStack.Pop();
                activeCommand.Execute();
                _undoStack.Push(activeCommand);
            }
        }

        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}