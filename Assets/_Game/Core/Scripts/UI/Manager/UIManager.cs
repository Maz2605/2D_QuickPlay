using System;
using _Game.Core.Scripts.Utils.DesignPattern.Singleton;
using UnityEngine;

namespace _Game.Core.Scripts.UI.Manager
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("--- Canvas Config ---")]
        [SerializeField] private Transform globalCanvasRoot; 

        [Header("--- Prefabs ---")]
        [SerializeField] private SettingBase settingsPrefab; 
        [SerializeField] private ConfirmationPopup confirmationPrefab;
        [SerializeField] private ToastNotification toastPrefab;
        [SerializeField] private GameObject loadingScreenPrefab;

        // --- Instances (Biến lưu bản cached) ---
        private SettingBase _settingInstance;
        private ConfirmationPopup _confirmInstance;
        private ToastNotification _toastInstance;
        private GameObject _loadingInstance;

        private float _savedTimeScale = 1f;
        
        private void Start()
        {
            InitUI();
        }

        private void InitUI()
        {
            // Kiểm tra và tạo Loading Screen ngay lập tức
            if (loadingScreenPrefab != null && _loadingInstance == null)
            {
                _loadingInstance = Instantiate(loadingScreenPrefab, globalCanvasRoot);
                _loadingInstance.SetActive(false);
            }
        }

       
        // SETTINGS SYSTEM
        public void OpenSettings()
        {
            if (_settingInstance == null)
            {
                if (settingsPrefab == null)
                {
                    Debug.LogError("UIManager: Chưa gắn Settings Prefab!");
                    return;
                }
                _settingInstance = Instantiate(settingsPrefab, globalCanvasRoot);
            }

            _savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            _settingInstance.OnClose.RemoveListener(OnSettingsClosed); 
            _settingInstance.OnClose.AddListener(OnSettingsClosed);

            _settingInstance.Show();
        }

        private void OnSettingsClosed()
        { 
            Time.timeScale = _savedTimeScale;
        }

        // CONFIRMATION SYSTEM
        public void ShowConfirmation(string title, string message, Action onConfirm, Action onCancel = null)
        {
            if (_confirmInstance == null)
            {
                if (confirmationPrefab == null) return;
                _confirmInstance = Instantiate(confirmationPrefab, globalCanvasRoot);
            }

            // _confirmInstance.Setup(title, message, onConfirm, onCancel);
            _confirmInstance.Show();
        }

        // TOAST SYSTEM
        public void ShowToast(string message, float duration = -1f)
        {
            if (_toastInstance == null)
            {
                if (toastPrefab == null) return;
                _toastInstance = Instantiate(toastPrefab, globalCanvasRoot);
                _toastInstance.transform.SetAsLastSibling(); // Đè lên tất cả
            }

            // Gọi hàm ShowToast custom vừa viết
            _toastInstance.ShowToast(message, duration);
        }

        // LOADING SYSTEM
        public void ToggleLoading(bool isShow)
        {
            if (_loadingInstance == null) InitUI(); // Tạo nếu chưa có
            
            if (_loadingInstance != null)
            {
                _loadingInstance.SetActive(isShow);
            }
        }
    }
}