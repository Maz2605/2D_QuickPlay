using System;
using _Game.Core.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.App.Scripts.MainMenu
{
    public class MiniGameItemView : MonoBehaviour
    {
        [SerializeField] private Image iconImg;
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private Button btn;

        private Action<GameProfileSO> _callback;
        private GameProfileSO _data;

        public void Setup(GameProfileSO data, Action<GameProfileSO> onClick)
        {
            _data = data;
            _callback = onClick;

            if(iconImg) iconImg.sprite = data.uiSprite;
            if (nameTxt)
            {
                nameTxt.text = data.displayName;
                nameTxt.color = data.textColor;
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => _callback?.Invoke(_data));
        }
    }
}