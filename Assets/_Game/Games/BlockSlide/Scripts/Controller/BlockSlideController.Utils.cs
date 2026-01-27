using System.Collections.Generic;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.GameSystem;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;

namespace _Game.Games.BlockSlide.Scripts.Controller
{
    public partial class BlockSlideController
    {
        private void ExecuteMove(BlockMoveDirection direction)
        {
            var command = new MoveBlockCommand(_gridModel, direction);
            _commandInvoker.ExecuteCommand(command);
            
            CheckGameOver();
        }

        private void RequestUndo()
        {
            if (_commandInvoker.CanUndo())
            {
                _commandInvoker.Undo();
            }
        }
        
        private void RestartGame()
        {
            _gridModel.ResetGame();
            _commandInvoker.ClearHistory();
            _scoreManager.ResetScore();
            _scoreManager.ForceUpdateEventUI(); 
            
            ChangeState(BlockSlideState.Playing);
        }

        private void CheckGameOver()
        {
            if (_gridModel.CheckIsGameOver())
            {
                OnGameOver();
            }
        }
        
        private void ChangeState(BlockSlideState newState)
        {
            _currentState = newState;
            EventManager<BlockSlideEventID>.Post(BlockSlideEventID.GameStateChanged, newState);
        }


        public override GlobalGameState CurrentGlobalState { get; }
        protected override void OnResetGameplay()
        {
            RestartGame();
        }

        
        private void OnScoreUpdate(int score)
        {
            _scoreManager.SyncScore(score);
        }

        private void OnGameOver()
        {
            ChangeState(BlockSlideState.GameOver);
            _scoreManager.UpdateLeaderboard();

            int finalScore = _gridModel.CurrentScore;
            
            List<int> topScores = _scoreManager.GetTopScores();
            
            GameOverPayLoad resultPayLoad = new GameOverPayLoad(finalScore, topScores);
            EventManager<BlockSlideEventID>.Post(BlockSlideEventID.ShowGameOver, resultPayLoad);
        }

        private void OnBoardUpdate()
        {
            if (boardView != null)
            {
                boardView.UpdateBoard(_gridModel);
            }
        }
    }
}