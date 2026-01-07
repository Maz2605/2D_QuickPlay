using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Core.Scripts.UI
{
    public class ConfirmationPopup: BasePopup
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backgroundButton;
        
        [Header("--- Animation ---")]
        [SerializeField] private Transform panelContent;
        
        private Action _onConfirmAction;
        private Action _onCancelAction;

        private void Start()
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            cancelButton.onClick.AddListener(OnCancelClicked);
            if(backgroundButton) backgroundButton.onClick.AddListener(OnCancelClicked);
        }

        public void Setup(string title, string message, Action onConfirm, Action onCancel = null)
        {
            if(titleText) titleText.text = title;
            if(messageText) messageText.text = message;

            onConfirm = onConfirm;
            onCancel = onCancel;
        }

        private void OnConfirmClicked()
        {
            _onConfirmAction?.Invoke();
            Hide();
        }

        private void OnCancelClicked()
        {
            _onCancelAction?.Invoke();
            Hide();
        }

        // --- ANIMATION (Override BasePopup) ---
        // Popup này sẽ phóng to từ giữa màn hình (Pop Up)
        protected override void PlayShowAnimation()
        {
            if (panelContent == null) return;
            panelContent.localScale = Vector3.zero;
            panelContent.DOScale(1f, animDuration).SetEase(Ease.OutBack).SetUpdate(true);
        }

        protected override void PlayHideAnimation(Action onComplete)
        {
            if (panelContent == null) { onComplete?.Invoke(); return; }
            panelContent.DOScale(0f, animDuration).SetEase(Ease.InBack).SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
    
}