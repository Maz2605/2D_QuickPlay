using _Game.Core.Scripts.Utils.DesignPattern.Command;
using _Game.Games.WaterSort.Scripts.Model;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public class MoveCommand : ICommand
    {
        private readonly BottleModel _fromBottle;
        private readonly BottleModel _toBottle;
        private readonly int _amount;
        private readonly int _colorId;

        public MoveCommand(BottleModel from, BottleModel to, int amount, int color)
        {
            _fromBottle = from;
            _toBottle = to;
            _amount = amount;
            _colorId = color;
        }

        public bool Execute()
        {
            if (_fromBottle.GetCountSameTopColor() < _amount) return false;
            
            for (int i = 0; i < _amount; i++)
            {
                _toBottle.Push(_fromBottle.Pop());
            }
            return true;
        }

        public void Undo()
        {
            for (int i = 0; i < _amount; i++)
            {
                _toBottle.Pop();
                _fromBottle.UndoPush(_colorId); 
            }
        }
    }
}