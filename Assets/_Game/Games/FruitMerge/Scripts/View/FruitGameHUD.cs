using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.Core.Scripts.GameSystem; 

namespace _Game.Games.FruitMerge.Scripts.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FruitGameHUD : MonoBehaviour
    {
        [Header("--- REFS ---")]
        [Tooltip("Kéo Prefab hoặc Object Result Panel vào đây")]
        [SerializeField] private FruitResultPanel resultPanel;
        
        [SerializeField] private CanvasGroup gameplayContainer; 

        [Header("--- BUTTONS ---")]
        [SerializeField] private Button btnHome;
        [SerializeField] private Button btnPause;
        [SerializeField] private Button btnReplay;
        
        [Header("--- DISPLAY ---")]
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtHighScore;
        
        [Header("--- COMBO EFFECT ---")]
        [SerializeField] private TextMeshProUGUI txtCombo;
        
        public event Action OnPauseClicked;
        public event Action OnReplayClicked;
        public event Action OnHomeClicked;

        private Tween _scoreTween;
        private Tween _comboTween;

        private void Awake()
        {
            if (txtCombo) txtCombo.gameObject.SetActive(false);
        }

        public void Initialize()
        {
            if (resultPanel != null)
            {
                resultPanel.Initialize(
                    onReplay: () => OnReplayClicked?.Invoke(),
                    onHome: () => OnHomeClicked?.Invoke()
                );
            }

            btnPause?.onClick.AddListener(() => OnPauseClicked?.Invoke());
            btnReplay?.onClick.AddListener(() => OnReplayClicked?.Invoke());
            btnHome?.onClick.AddListener(() => OnHomeClicked?.Invoke());

            SetUIState(GlobalGameState.Playing);
        }

        private void OnDestroy()
        {
            btnPause?.onClick.RemoveAllListeners();
            btnReplay?.onClick.RemoveAllListeners();
            btnHome?.onClick.RemoveAllListeners();

            _scoreTween?.Kill();
            _comboTween?.Kill();
            transform.DOKill();
        }


        public void SetUIState(GlobalGameState state, int finalScore = 0, List<int> topScores = null)
        {
            switch (state)
            {
                case GlobalGameState.Playing:
                case GlobalGameState.Loading:
                    FadeContainer(gameplayContainer, 1f, true);
                    if(resultPanel) resultPanel.Hide();
                    break;

                case GlobalGameState.Paused:
                    FadeContainer(gameplayContainer, 0.5f, false);
                    break;

                case GlobalGameState.GameOver:
                    FadeContainer(gameplayContainer, 0f, false);
                    if (resultPanel) resultPanel.Show(false, finalScore, topScores); 
                    break;

                case GlobalGameState.Resetting:
                    FadeContainer(gameplayContainer, 1f, false);
                    if(resultPanel) resultPanel.Hide();
                    break;
            }
        }

        private void FadeContainer(CanvasGroup cg, float alpha, bool interactable)
        {
            if (cg == null) return;
            cg.DOKill(); 
            cg.DOFade(alpha, 0.3f).SetLink(cg.gameObject);
            cg.interactable = interactable;
            cg.blocksRaycasts = interactable;
        }

        public void UpdateScore(int score)
        {
            if (txtScore == null) return;
            txtScore.text = score.ToString();

            _scoreTween?.Complete(); 
            _scoreTween = txtScore.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 0)
                .SetLink(txtScore.gameObject);
        }

        public void UpdateHighScore(int highScore)
        {
            if (txtHighScore) txtHighScore.text = highScore.ToString();
        }

        public void ShowCombo(int comboMultiplier)
        {
            if (txtCombo == null || comboMultiplier <= 1) return;

            _comboTween?.Kill();
    
            txtCombo.gameObject.SetActive(true);
            txtCombo.text = $"COMBO x{comboMultiplier}!";
            
            txtCombo.transform.localScale = Vector3.zero;
            txtCombo.alpha = 1f;

            Sequence seq = DOTween.Sequence();
            
            seq.Append(txtCombo.transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack));
            seq.AppendInterval(0.5f);
            seq.Append(txtCombo.transform.DOLocalMoveY(txtCombo.transform.localPosition.y + 100f, 0.4f));
            seq.Join(txtCombo.DOFade(0f, 0.4f));
            seq.OnComplete(() =>
            {
                txtCombo.gameObject.SetActive(false);
                txtCombo.transform.localPosition -= new Vector3(0, 100f, 0);
            });

            seq.SetLink(gameObject);
            _comboTween = seq;
        }
    }
}