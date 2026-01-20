using System;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.UI.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.App.Scripts.MainMenu
{
    public class MainMenuHUD : MonoBehaviour
    {
        [Header("Coin Display")]
        [SerializeField] private TextMeshProUGUI coinText;

        [Header("Buttons")]
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button removeAdsBtn;
        
        
        public event Action OnPauseClicked;
        
        private void Start()
        {
            UpdateCoinUI();
        
            settingBtn.onClick.AddListener(OnSettingClicked);
            removeAdsBtn.onClick.AddListener(OnRemoveAdsClicked);
        }

        public void UpdateCoinUI()
        {
            int currentCoin = PlayerPrefs.GetInt("USER_COIN", 0); 
            coinText.text = currentCoin.ToString();
        }

        private void OnSettingClicked()
        {
            Debug.Log("Open Setting Popup");
            UIManager.Instance.OpenSettings();
        }

        private void OnRemoveAdsClicked()
        {
            Debug.Log("Buy Remove Ads");
            // Gọi IAP ở đây
        }
    }
}
