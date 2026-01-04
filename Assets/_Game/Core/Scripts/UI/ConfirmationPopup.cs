using System;
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
        [SerializeField] private TextMeshProUGUI confirmLabel;
        
        private Action _onConfirmAction;
        private Action _onCancelAction;

        private void Start()
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            cancelButton.onClick.AddListener(OnCancelClicked);
        }

        public void Show(string title, string message, Action onConfirmAction, Action onCancelAction)
        {
            if(titleText) titleText.text = title;
            if (messageText) messageText.text = message;
            if(confirmLabel) confirmLabel.text = message;
            
            _onConfirmAction = onConfirmAction;
            _onCancelAction = onCancelAction;
            base.Show();
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
    }
}