using UnityEngine;
using _Game.Core.Scripts.GameSystem;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public partial class WaterSortController
    {
        private void RegisterUIEvents()
        {
            if (gameHUD)
            {
                gameHUD.Initialize();
                gameHUD.OnUndoClicked += HandleUndoRequest;
                gameHUD.OnReplayClicked += HandleReplayRequest;
                gameHUD.OnHintClicked += HandleHintRequest;
                gameHUD.OnPauseClicked += HandlePauseRequest;
                gameHUD.OnHomeClicked += HandleHomeRequest;
                gameHUD.OnWinAnimationCovered += HandleLoadNextLevel;
            }
        }

        private void UnregisterUIEvents()
        {
            if (gameHUD)
            {
                gameHUD.OnUndoClicked -= HandleUndoRequest;
                gameHUD.OnReplayClicked -= HandleReplayRequest;
                gameHUD.OnHintClicked -= HandleHintRequest;
                gameHUD.OnPauseClicked -= HandlePauseRequest;
                gameHUD.OnHomeClicked -= HandleHomeRequest;
                gameHUD.OnWinAnimationCovered -= HandleLoadNextLevel;
            }
        }

        private void UpdateHUDInfo()
        {
            if (gameHUD && LevelManager.HasInstance)
                gameHUD.UpdateLevelName(LevelManager.Instance.GetCurrentLevelName());
        }

        // --- HANDLERS ---
        private void HandleUndoRequest()
        {
            if (_isPaused || _localState != WaterSortLocalState.Idle || _undoStack.Count == 0 || _isBusy) return;
            ExecuteUndo();
        }

        private void HandleReplayRequest() => RequestReplay();
        private void HandleHintRequest()
        {
            if (!_isPaused && !_isBusy) ShowHint();
        }

        private void HandlePauseRequest()
        {
            RequestPause();
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
            if (gameHUD) gameHUD.SetPauseState(_isPaused);
        }

        private void HandleHomeRequest() => RequestBackHome();

        private void HandleLoadNextLevel()
        {
            if (LevelManager.HasInstance)
            {
                LevelManager.Instance.LoadNextLevel();
                UpdateHUDInfo(); 
            }
        }
    }
}