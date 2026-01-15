using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Games.FruitMerge.Scripts.View
{
    public class FruitGameHUD : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button btnHome;
        [SerializeField] private Button btnPause;
        [SerializeField] private Button btnReplay;
        
        [Header("Display")]
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtHighScore;
        
        [Header("Combo Effect")]
        [SerializeField] private TextMeshProUGUI txtCombo;
        
        public event Action OnPauseClicked;
        public event Action OnReplayClicked;
        public event Action OnHomeClicked;
        
        private Tween _comboTween;

        private void Awake()
        {
            if(txtCombo) txtCombo.gameObject.SetActive(false);
        }
        private void Start()
        {
            btnPause?.onClick.AddListener(() => OnPauseClicked?.Invoke());
            btnReplay?.onClick.AddListener(() => OnReplayClicked?.Invoke());
            btnHome?.onClick.AddListener(() => OnHomeClicked?.Invoke());
        }
        
        public void ShowCombo(int comboMultiplier)
        {
            if (txtCombo == null) return;

            _comboTween?.Kill();
    
            txtCombo.gameObject.SetActive(true);
            txtCombo.text = $"COMBO x{comboMultiplier}!";
    
            txtCombo.transform.localScale = Vector3.one; 

            _comboTween = DOTween.Sequence()
                .Append(txtCombo.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 10, 1))
                .AppendInterval(0.5f)
                .Append(txtCombo.transform.DOScale(0f, 0.2f))
                .OnComplete(() => txtCombo.gameObject.SetActive(false));
        }
        public void UpdateScore(int score) => txtScore.text = score.ToString();
        public void UpdateHighScore(int highScore) => txtHighScore.text = highScore.ToString();
    }
}