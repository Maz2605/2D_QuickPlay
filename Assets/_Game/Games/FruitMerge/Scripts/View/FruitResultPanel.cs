using System;
using System.Collections.Generic; // Cần thiết để dùng List
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace _Game.Games.FruitMerge.Scripts.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FruitResultPanel : MonoBehaviour
    {
        [Header("--- COMPONENTS ---")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI txtTitle;       
        [SerializeField] private TextMeshProUGUI txtFinalScore;  

        [Header("--- LEADERBOARD ---")]
        [Tooltip("Kéo 3 cái Text điểm số (Top 1, 2, 3) trong hình vào đây theo thứ tự")]
        [SerializeField] private List<TextMeshProUGUI> txtTopScores; 
        
        [Header("--- VISUAL SETTINGS ---")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightColor = Color.yellow; 

        [Header("--- BUTTONS ---")]
        [SerializeField] private Button btnReplay;
        [SerializeField] private Button btnHome;
        
        private Action _onReplay;
        private Action _onHome;
        

        public void Initialize(Action onReplay, Action onHome)
        {
            _onReplay = onReplay;
            _onHome = onHome;
            
            btnReplay.onClick.RemoveAllListeners();
            btnHome.onClick.RemoveAllListeners();

            btnReplay.onClick.AddListener(() => _onReplay?.Invoke());
            btnHome.onClick.AddListener(() => _onHome?.Invoke());
            
            HideInstant();
        }

        // --- DISPLAY LOGIC ---

        public void Show(bool isWin, int currentScore, List<int> topScores)
        {
            gameObject.SetActive(true);
            
            txtTitle.text = isWin ? "FANTASTIC!" : "GAME OVER";
            txtFinalScore.text = currentScore.ToString();
            
            UpdateLeaderboardUI(currentScore, topScores);
            
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 0;
            transform.localScale = Vector3.one * 0.8f;

            Sequence seq = DOTween.Sequence();
            seq.Append(canvasGroup.DOFade(1f, 0.3f));
            seq.Join(transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            
            seq.SetLink(gameObject); 
        }

        private void UpdateLeaderboardUI(int currentScore, List<int> data)
        {
            if (data == null) data = new List<int>();
            
            for (int i = 0; i < txtTopScores.Count; i++)
            {
                if (txtTopScores[i] == null) continue;

                if (i < data.Count)
                {
                    int score = data[i];
                    txtTopScores[i].text = score.ToString();
                    
                    if (score == currentScore && currentScore > 0)
                    {
                        txtTopScores[i].color = highlightColor;
                    }
                    else
                    {
                        txtTopScores[i].color = normalColor;
                    }
                }
                else
                {
                    txtTopScores[i].text = "0";
                    txtTopScores[i].color = normalColor;
                }
            }
        }

        // --- HIDE LOGIC ---

        public void Hide()
        {
            canvasGroup.blocksRaycasts = false;
            
            canvasGroup.DOFade(0f, 0.2f)
                .OnComplete(() => gameObject.SetActive(false))
                .SetLink(gameObject);
        }

        private void HideInstant()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            btnReplay?.onClick.RemoveAllListeners();
            btnHome?.onClick.RemoveAllListeners();
            transform.DOKill(); 
        }
    }
}