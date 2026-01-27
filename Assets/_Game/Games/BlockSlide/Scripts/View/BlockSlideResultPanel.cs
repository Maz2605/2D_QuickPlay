using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Games.BlockSlide.Scripts.View
{
    public class BlockSlideResultPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI txtFinalScore;
        [SerializeField] private List<TextMeshProUGUI> topScores;
        
        [Header("Buttons")]
        [SerializeField] private Button btnReplay;
        [SerializeField] private Button btnHome;

        public void Initialize(Action onBackHome, Action onReplay)
        {
            btnHome?.onClick.AddListener(()=>onBackHome?.Invoke());
            btnReplay?.onClick.AddListener(()=>onReplay?.Invoke());
            
            gameObject.SetActive(false);
        }

        public void ShowResults(int currentScore, List<int> topScores)
        {
            gameObject.SetActive(true);
            
            if (txtFinalScore) txtFinalScore.text = currentScore.ToString();

            for (int i = 0; i < this.topScores.Count; i++)
            {
                if (this.topScores[i] == null) continue;

                if (i < topScores.Count)
                {
                    this.topScores[i].text = topScores[i].ToString();
                    this.topScores[i].gameObject.SetActive(true);
                }
                else
                {
                    this.topScores[i].text = "---";
                }
            }
        }

        public void HideResults()
        {
            this.gameObject.SetActive(false);
        }

    }
}