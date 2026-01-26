using _Game.Core.Scripts.Utils.DesignPattern.Command;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Games.BlockSlide.Scripts.Controller
{
    public partial class BlockSlideController
    {
        [Header("Command Patterns")]
        private CommandInvoker _commandInvoker;
        [SerializeField] private int undoLimit = 3;

        private void ExecuteMove(BlockMoveDirection direction)
        {
            var command = new MoveBlockCommand(_gridModel, direction);
            
            _commandInvoker.ExecuteCommand(command);
        }
    }
}