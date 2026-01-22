using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace _Game.Games.WaterSort.Scripts.View
{
    public class WaterSortHUD : MonoBehaviour
    {
        [Header("--- BUTTONS ---")]
        [SerializeField] private Button btnUndo;
        [SerializeField] private Button btnReplay;
        [SerializeField] private Button btnHint; 
        [SerializeField] private Button btnHome; 
        [SerializeField] private Button btnPause; 
        
        [Header("--- TEXT DISPLAY ---")]
        [SerializeField] private TextMeshProUGUI txtLevelName;

        [Header("--- WIN SCREEN ANIMATION ---")]
        [SerializeField] private RectTransform winPanel; 
        [SerializeField] private CanvasGroup bgDim; 
        [SerializeField] private Transform winLevelTextContainer; 
        [SerializeField] private TextMeshProUGUI txtWinLevelName;

        [Header("--- ANIM CONFIG ---")]
        [SerializeField] private float slideDuration = 0.8f;
        [SerializeField] private float stayDuration = 2.0f;
        [SerializeField] private Ease enterEase = Ease.OutExpo;
        [SerializeField] private Ease exitEase = Ease.InBack;

        public event Action OnUndoClicked;
        public event Action OnReplayClicked;
        public event Action OnHintClicked;
        public event Action OnHomeClicked;
        public event Action OnPauseClicked;
        
        public event Action OnWinAnimationCovered; 

        private float _screenWidth;
        private Tween _transitionTween;

        private void Awake()
        {
            _screenWidth = Screen.width; 
            
            if (winPanel) 
            {
                winPanel.gameObject.SetActive(false); 
                winPanel.anchoredPosition = new Vector2(_screenWidth, 0); 
            }
            if (bgDim) 
            {
                bgDim.gameObject.SetActive(false); 
                bgDim.alpha = 0; 
            }
        }

        public void Initialize()
        {
            btnUndo?.onClick.AddListener(() => OnUndoClicked?.Invoke());
            btnReplay?.onClick.AddListener(() => OnReplayClicked?.Invoke());
            btnHint?.onClick.AddListener(() => OnHintClicked?.Invoke());
            btnHome?.onClick.AddListener(() => OnHomeClicked?.Invoke());
            btnPause?.onClick.AddListener(() => OnPauseClicked?.Invoke());
        }

        private void OnDestroy()
        {
            btnUndo?.onClick.RemoveAllListeners();
            btnReplay?.onClick.RemoveAllListeners();
            btnHint?.onClick.RemoveAllListeners();
            btnHome?.onClick.RemoveAllListeners();
            btnPause?.onClick.RemoveAllListeners();

            _transitionTween?.Kill();
            if (winPanel) winPanel.DOKill();
        }


        public void UpdateLevelName(string name)
        {
            if (txtLevelName) txtLevelName.text = name;
        }

        public void SetPauseState(bool isPaused)
        {
            Debug.Log($"UI Paused State: {isPaused}");
        }

        
        public void PlayWinSequence(string completedLevelName)
        {
            if (winPanel == null) return;

            winPanel.gameObject.SetActive(true);
            winPanel.anchoredPosition = new Vector2(_screenWidth, 0);
            
            if (bgDim) { bgDim.gameObject.SetActive(true); bgDim.alpha = 0; }
            
            if (txtWinLevelName) txtWinLevelName.text = "COMPLETED\n" + completedLevelName;
            if (winLevelTextContainer) winLevelTextContainer.localScale = Vector3.zero;

            _transitionTween?.Kill();
            Sequence seq = DOTween.Sequence();

            if (bgDim) seq.Append(bgDim.DOFade(1f, 0.3f));
            seq.Join(winPanel.DOAnchorPosX(0, slideDuration).SetEase(enterEase));
            if (winLevelTextContainer) seq.Append(winLevelTextContainer.DOScale(1f, 0.6f).SetEase(Ease.OutElastic));

            seq.AppendInterval(stayDuration);

            seq.AppendCallback(() => {
                OnWinAnimationCovered?.Invoke(); 
            });

            if (winLevelTextContainer) seq.Append(winLevelTextContainer.DOScale(0f, 0.2f).SetEase(Ease.InBack));
            seq.Join(winPanel.DOAnchorPosX(-_screenWidth, 0.6f).SetEase(exitEase));
            if (bgDim) seq.Join(bgDim.DOFade(0f, 0.4f));

            seq.OnComplete(() => {
                winPanel.gameObject.SetActive(false);
                if (bgDim) bgDim.gameObject.SetActive(false);
                winPanel.anchoredPosition = new Vector2(_screenWidth, 0);
            });
            
            _transitionTween = seq;
        }
    }
}