using System;
using System.Collections;
using _Script.DesignPattern.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.Core.Scripts.UI
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("Global Popups")]
        [SerializeField] private SettingPopup settingPopup;
        [SerializeField] private ConfirmationPopup confirmationPopup;
        [SerializeField] private ToastNotification toastNotification;
        [SerializeField] private GameObject loadingScreen;
        
        private float _saveTimeScale = 1.0f;

        
        //===Settings System===
        public void ShowSettings()
        {
            if(settingPopup == null) return;
            
            _saveTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            
            settingPopup.Show();
        }

        public void OnSettingsClosed()
        {
            Time.timeScale = _saveTimeScale;
        }

        public void ShowLoadingScreen(bool isShow)
        {
            if(loadingScreen) loadingScreen.SetActive(isShow);
        }
        
        //===Confirmation System===
        public void ShowConfirmation(string title, string message, Action onConfirm, Action onCancel)
        {
            if(confirmationPopup == null) return;
            confirmationPopup.Show(title, message, onConfirm, onCancel);
        }
        
        //===Toast System===
        public void ShowToastNotification(string message)
        {
            if(toastNotification == null) return;
            toastNotification.Show(message);
        }
        
        //===Scene Loading System===

        public void ToggleLoadingScreen(bool isShow)
        {
            if(loadingScreen) loadingScreen.SetActive(isShow);
        }
        
        
        
        
    }
}
