using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.UI.Base;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.FruitMerge.Scripts.Config;

namespace _Game.Games.FruitMerge.Scripts.View
{
    public class FruitGameHUD : BaseHUD
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            EventManager<FruitMergeEventID>.AddListener<FruitMergeState>(FruitMergeEventID.GameStateChanged, OnGameStateChanged);
            EventManager<FruitMergeEventID>.AddListener<int>(FruitMergeEventID.ScoreUpdated, OnScoreUpdated);
            EventManager<FruitMergeEventID>.AddListener<int>(FruitMergeEventID.HighScoreUpdated, OnHighScoreUpdated);
            EventManager<FruitMergeEventID>.AddListener<GameOverPayLoad>(FruitMergeEventID.ShowGameOver, OnShowGameOver);
        }

        protected override void OnDisable()
        {
            EventManager<FruitMergeEventID>.RemoveListener<FruitMergeState>(FruitMergeEventID.GameStateChanged, OnGameStateChanged);
            EventManager<FruitMergeEventID>.RemoveListener<int>(FruitMergeEventID.ScoreUpdated, OnScoreUpdated);
            EventManager<FruitMergeEventID>.RemoveListener<int>(FruitMergeEventID.HighScoreUpdated, OnHighScoreUpdated);
            EventManager<FruitMergeEventID>.RemoveListener<GameOverPayLoad>(FruitMergeEventID.ShowGameOver, OnShowGameOver);
            base.OnDisable();
        }

        private void OnShowGameOver(GameOverPayLoad result)
        {
            ShowResultPanel(result.FinalScore, result.TopScores);
        }

        private void OnHighScoreUpdated(int score)
        {
            AnimateScoreText(txtHighScore, score, ref ScoreTween);
        }

        private void OnScoreUpdated(int score)
        {
            AnimateScoreText(txtCurrentScore, score, ref ScoreTween);
        }

        private void OnGameStateChanged(FruitMergeState state)
        {
            switch (state)
            {
                case FruitMergeState.Playing:
                    HideResultPanel();
                    ShowPanel(gamePlayPanel, true);
                    break;
                case FruitMergeState.GameOver:
                    ShowPanel(gamePlayPanel, false);
                    break;
            }
        }
    }
}