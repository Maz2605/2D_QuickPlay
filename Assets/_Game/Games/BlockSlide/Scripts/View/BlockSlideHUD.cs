using System;
using System.Collections.Generic;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.Games.BlockSlide.Scripts.View
{
    public class BlockSlideHUD : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button btnHome;
        [SerializeField] private Button btnReplay;
        [SerializeField] private Button btnPause;
        [SerializeField] private Button btnUndo;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI txtCurrentScore;
        [SerializeField] private TextMeshProUGUI txtHighScore;
        
        [Header("Components")]
        [SerializeField] private BlockSlideResultPanel resultPanel;
        [SerializeField] private CanvasGroup gamePlayPanel;
        
        public Action OnUndoClicked;
        public Action OnPauseClicked;
        public Action OnReplayClicked;
        public Action OnHomeClicked;
        
        private void OnEnable()
        {
            EventManager<BlockSlideEventID>.AddListener<BlockSlideState>(BlockSlideEventID.GameStateChanged, OnGameStateChanged);
            EventManager<BlockSlideEventID>.AddListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
            EventManager<BlockSlideEventID>.AddListener<int>(BlockSlideEventID.HighScoreUpdate, OnHighScoreUpdate);
            EventManager<BlockSlideEventID>.AddListener<GameOverPayLoad>(BlockSlideEventID.ShowGameOver, OnShowGameOver);
        }


        private void OnDisable()
        {
            EventManager<BlockSlideEventID>.RemoveListener<BlockSlideState>(BlockSlideEventID.GameStateChanged, OnGameStateChanged);
            EventManager<BlockSlideEventID>.RemoveListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
            EventManager<BlockSlideEventID>.RemoveListener<int>(BlockSlideEventID.HighScoreUpdate, OnHighScoreUpdate);
        }

        private void OnHighScoreUpdate(int score)
        {
            txtHighScore.text = score.ToString();
        }

        private void OnGameStateChanged(BlockSlideState state)
        {
            switch (state)
            {
                case BlockSlideState.Playing:
                    HideResultPanel();
                    // FadeContainer(gamePlayPanel,1f, false);
                    break;
                case BlockSlideState.Paused:
                case BlockSlideState.GameOver:
                    // FadeContainer(gamePlayPanel, 0.5f, false);
                    break;
            }
        }

        private void OnScoreUpdate(int score)
        {
            txtCurrentScore.text = score.ToString();
        }

        private void OnShowGameOver(GameOverPayLoad resultPayLoad)
        {
            ShowResultPanel(resultPayLoad.FinalScore, resultPayLoad.TopScores);
        }
        public void Initialize()
        {
            btnHome?.onClick.AddListener(() => OnHomeClicked?.Invoke());
            btnReplay?.onClick.AddListener(() => OnReplayClicked?.Invoke());
            btnPause?.onClick.AddListener(() => OnPauseClicked?.Invoke());
            btnUndo?.onClick.AddListener(() => OnUndoClicked?.Invoke());

            if (resultPanel != null)
            {
                resultPanel.Initialize(
                    () => OnHomeClicked?.Invoke(),
                    () => OnReplayClicked?.Invoke()
                    );
            }
        }

        public void ShowResultPanel(int score, List<int> topScores)
        {
            if(resultPanel == null) return;
            
            resultPanel.ShowResults(score, topScores);
        }

        public void HideResultPanel()
        {
            resultPanel.HideResults();
        }
        private void FadeContainer(CanvasGroup cg, float alpha, bool interactable)
        {
            if (cg == null) return;
            cg.DOKill(); 
            cg.DOFade(alpha, 0.3f).SetLink(cg.gameObject);
            cg.interactable = interactable;
            cg.blocksRaycasts = interactable;
        }
    }
}