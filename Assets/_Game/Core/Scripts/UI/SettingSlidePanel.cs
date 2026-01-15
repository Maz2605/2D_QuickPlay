using System;
using _Game.Core.Scripts.UI.Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Scripts.UI
{
    public class SettingSlidePanel : SettingBase
    {
        [Header("--- Slide Config (Visual) ---")]
        [SerializeField] private RectTransform contentPanel; 
        [SerializeField] private Ease slideEase = Ease.OutBack;
        [SerializeField] private bool slideFromRight = true;

        private float _screenWidth;

        protected override void Awake()
        {
            base.Awake();
            _screenWidth = Screen.width;
        }

        // --- OVERRIDE ANIMATION ---

        protected override void PlayShowAnimation()
        {
            if (contentPanel == null) return;

            float startX = slideFromRight ? _screenWidth : -_screenWidth;
            contentPanel.anchoredPosition = new Vector2(startX, 0);

            contentPanel.DOAnchorPosX(0, animDuration)
                .SetEase(slideEase)
                .SetUpdate(true);
        }

        protected override void PlayHideAnimation(System.Action onComplete)
        {
            if (contentPanel == null)
            {
                onComplete?.Invoke();
                return;
            }

            float exitX = slideFromRight ? _screenWidth : -_screenWidth;

            contentPanel.DOAnchorPosX(exitX, animDuration)
                .SetEase(Ease.InBack) 
                .SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}
        