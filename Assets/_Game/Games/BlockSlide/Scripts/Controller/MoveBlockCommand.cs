using _Game.Core.Scripts.Utils.DesignPattern.Command;
using _Game.Games.BlockSlide.Scripts.Model;

namespace _Game.Games.BlockSlide.Scripts.Controller
{
    public class MoveBlockCommand : ICommand
    {
        private GridModel _gridModel;
        private BlockMoveDirection _direction;
        private BoardSnapshot _previousState;

        public MoveBlockCommand(GridModel gridModel, BlockMoveDirection direction)
        {
            _gridModel = gridModel;
            _direction = direction;
        }

        public bool Execute()
        {
            _previousState = _gridModel.CaptureState();
            _gridModel.MoveBlock(_direction);

            return _gridModel.IsLastMoveChanged;
        }

        public void Undo()
        {
            _gridModel.RestoreState(_previousState);
        }
    }
}