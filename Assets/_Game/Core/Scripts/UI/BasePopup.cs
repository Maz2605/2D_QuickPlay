using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Core.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BasePopup : MonoBehaviour
    {
        [Header("Base Settings")] [SerializeField]
        protected CanvasGroup canvasGroup;

        [SerializeField] protected Transform content;
        [SerializeField] protected float animDuration = 0.2f;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        protected virtual void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnOpen?.Invoke();

            canvasGroup.alpha = 0f;
            content.localScale = Vector3.one;

            canvasGroup.DOKill();
            content.DOKill();

            canvasGroup.DOFade(1f, animDuration).SetUpdate(true);
            content.DOScale(1f, animDuration).SetEase(Ease.OutBack).SetUpdate(true);
        }

        public virtual void Hide()
        {
            canvasGroup.DOKill();
            content.DOKill();

            canvasGroup.DOFade(0f, animDuration).SetUpdate(true);
            content.DOScale(1f, animDuration).SetEase(Ease.InBack).SetUpdate(true)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    OnClose?.Invoke();
                });
        }
    }
}