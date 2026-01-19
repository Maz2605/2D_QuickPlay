using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Core.Scripts.SkillSystem
{
    public class UISkillSlot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button btnSkill;
        [SerializeField] private Image imgIcon;
        [SerializeField] private Image imgCooldownOverlay;
        [SerializeField] private TextMeshProUGUI txtPrice;

        private BaseSkillSO _data;
        private Action<BaseSkillSO> _onClickCallback;
        private float _currentCooldown;
        private bool _isInteractable = true;

        public void Initialize(BaseSkillSO data, Action<BaseSkillSO> onClick)
        {
            _data = data;
            _onClickCallback = onClick;

            if (imgIcon) imgIcon.sprite = data.icon;
            if (txtPrice) txtPrice.text = data.price > 0 ? data.price.ToString() : "FREE";

            btnSkill.onClick.RemoveAllListeners();
            btnSkill.onClick.AddListener(() =>
            {
                if (_currentCooldown <= 0 && _isInteractable)
                    _onClickCallback?.Invoke(_data);
            });
            
            ResetState();
        }

        public void StartCooldown()
        {
            _currentCooldown = _data.cooldownTime;
            UpdateVisual();
        }

        public void ResetState()
        {
            _currentCooldown = 0;
            UpdateVisual();
        }

        private void Update()
        {
            if (_currentCooldown > 0)
            {
                _currentCooldown -= Time.deltaTime;
                UpdateVisual();
            }
        }

        private void UpdateVisual()
        {
            float fill = 0;
            if (_data != null && _data.cooldownTime > 0)
                fill = _currentCooldown / _data.cooldownTime;

            if (imgCooldownOverlay) imgCooldownOverlay.fillAmount = fill;
            
            // Logic làm mờ nút
            if (btnSkill) btnSkill.interactable = _currentCooldown <= 0 && _isInteractable;
        }
    }
}