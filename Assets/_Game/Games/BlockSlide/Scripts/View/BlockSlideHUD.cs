using System;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.UI.Base;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Games.BlockSlide.Scripts.View
{
    public class BlockSlideHUD : BaseHUD
    {
        [Header("Skill buttons")]
        [SerializeField] private Button btnUndo;
        
        public Action OnUndoClicked;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            EventManager<BlockSlideEventID>.AddListener<BlockSlideState>(BlockSlideEventID.GameStateChanged, OnGameStateChanged);
            EventManager<BlockSlideEventID>.AddListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
            EventManager<BlockSlideEventID>.AddListener<int>(BlockSlideEventID.HighScoreUpdate, OnHighScoreUpdate);
            EventManager<BlockSlideEventID>.AddListener<GameOverPayLoad>(BlockSlideEventID.ShowGameOver, OnShowGameOver);
            
        }

        protected override void OnDisable()
        {
            EventManager<BlockSlideEventID>.RemoveListener<BlockSlideState>(BlockSlideEventID.GameStateChanged, OnGameStateChanged);
            EventManager<BlockSlideEventID>.RemoveListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
            EventManager<BlockSlideEventID>.RemoveListener<int>(BlockSlideEventID.HighScoreUpdate, OnHighScoreUpdate);
            EventManager<BlockSlideEventID>.RemoveListener<GameOverPayLoad>(BlockSlideEventID.ShowGameOver, OnShowGameOver);
                
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            btnUndo?.onClick.RemoveAllListeners();
        }

        public override void Initialize()
        {
            base.Initialize();
            BindButton(btnUndo, () => OnUndoClicked?.Invoke());
        }
        

        private void OnScoreUpdate(int score) => AnimateScoreText(txtCurrentScore, score, ref ScoreTween);
        
        private void OnHighScoreUpdate(int score) => AnimateScoreText(txtHighScore, score, ref HighScoreTween);

        private void OnGameStateChanged(BlockSlideState state)
        {
            switch (state)
            {
                case BlockSlideState.Playing:
                    HideResultPanel();
                    ShowPanel(gamePlayPanel, true); 
                    break;
                case BlockSlideState.Paused:
                    break;
                case BlockSlideState.GameOver:
                    ShowPanel(gamePlayPanel, false);
                    break;
            }
        }

        private void OnShowGameOver(GameOverPayLoad resultPayLoad)
        {
            ShowResultPanel(resultPayLoad.FinalScore, resultPayLoad.TopScores);
        }
    }
}