using System;
using System.Collections.Generic;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.UI;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
        [SerializeField] private GameOverResultPanel resultPanel;
        [SerializeField] private CanvasGroup gamePlayPanel; 
        
        public Action OnUndoClicked;
        public Action OnPauseClicked;
        public Action OnReplayClicked;
        public Action OnReplayWithoutPopupClicked;
        public Action OnHomeWithoutPopupClicked;
        public Action OnHomeClicked;
        
        private Tween _scoreTween;
        private Tween _highScoreTween;

        private void OnEnable()
        {
            EventManager<BlockSlideEventID>.AddListener<BlockSlideState>(BlockSlideEventID.GameStateChanged, OnGameStateChanged);
            EventManager<BlockSlideEventID>.AddListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
            EventManager<BlockSlideEventID>.AddListener<int>(BlockSlideEventID.HighScoreUpdate, OnHighScoreUpdate);
            EventManager<BlockSlideEventID>.AddListener<GameOverPayLoad>(BlockSlideEventID.ShowGameOver, OnShowGameOver);
            
            ShowPanel(gamePlayPanel, true);
        }

        private void OnDisable()
        {
            EventManager<BlockSlideEventID>.RemoveListener<BlockSlideState>(BlockSlideEventID.GameStateChanged, OnGameStateChanged);
            EventManager<BlockSlideEventID>.RemoveListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
            EventManager<BlockSlideEventID>.RemoveListener<int>(BlockSlideEventID.HighScoreUpdate, OnHighScoreUpdate);
            
            transform.DOKill();
        }

        private void OnDestroy()
        {
            btnHome?.onClick.RemoveAllListeners();
            btnReplay?.onClick.RemoveAllListeners();
            btnPause?.onClick.RemoveAllListeners();
            btnUndo?.onClick.RemoveAllListeners();
        }

        public void Initialize()
        {
            BindButton(btnHome,() => OnHomeClicked?.Invoke());
            BindButton(btnPause, () => OnPauseClicked?.Invoke());
            BindButton(btnReplay, () => OnReplayClicked?.Invoke());
            BindButton(btnUndo, () => OnUndoClicked?.Invoke());

            if (resultPanel != null)
            {
                resultPanel.Initialize(
                    () => OnHomeWithoutPopupClicked?.Invoke(),
                    () => OnReplayWithoutPopupClicked?.Invoke()
                );
            }
        }

        // --- CORE ANIMATION LOGIC ---

        private void AnimateScoreText(TextMeshProUGUI tmp, int score, ref Tween tweenRef)
        {
            if (tmp == null) return;

            tmp.SetText("{0}", score);

            if (tweenRef != null && tweenRef.IsActive()) tweenRef.Kill();

            tmp.transform.localScale = Vector3.one;

            tweenRef = tmp.transform
                .DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 1f)
                .SetEase(Ease.OutQuad)
                .SetLink(tmp.gameObject);
        }
        
        private void BindButton(Button btn, Action onClickAction)
        {
            if (btn == null) return;
            
            btn.onClick?.RemoveAllListeners();
            btn.onClick?.AddListener(() =>
            {
                btn.transform.DOKill(true);
                btn.transform.localScale = Vector3.one;
                btn.transform
                    .DOPunchScale(Vector3.one * -0.1f, 0.15f, 5, 1) 
                    .SetUpdate(true) 
                    .OnComplete(() => onClickAction?.Invoke());
            });
        }

        
        private void ShowPanel(CanvasGroup cg, bool isShow)
        {
            if (cg == null) return;

            cg.DOKill();
            cg.transform.DOKill();

            if (isShow)
            {
                cg.alpha = 0f;
                cg.transform.localScale = Vector3.one * 0.8f; 
                cg.interactable = true;
                cg.blocksRaycasts = true;

                cg.DOFade(1f, 0.3f).SetUpdate(true);
                cg.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
            }
            else
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;

                cg.DOFade(0f, 0.2f).SetUpdate(true);
                cg.transform.DOScale(0.9f, 0.2f).SetEase(Ease.InBack).SetUpdate(true);
            }
        }

        // --- EVENT HANDLERS ---

        private void OnScoreUpdate(int score) => AnimateScoreText(txtCurrentScore, score, ref _scoreTween);
        
        private void OnHighScoreUpdate(int score) => AnimateScoreText(txtHighScore, score, ref _highScoreTween);

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

        public void ShowResultPanel(int score, List<int> topScores)
        {
            if(resultPanel == null) return;
            resultPanel.ShowResults(score, topScores);
        }

        public void HideResultPanel()
        {
            if (resultPanel != null) resultPanel.HideResults();
        }
    }
}