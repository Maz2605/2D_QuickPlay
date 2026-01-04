using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Game.Core.Scripts.UI
{
    public class ToastNotification : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private RectTransform bgRectTransform;

        public void Show(string message)
        {
            messageText.text = message;
            gameObject.SetActive(true);
            
            canvasGroup.alpha = 1;
            bgRectTransform.localScale = Vector3.one * 0.5f;
            
            canvasGroup.DOKill();
            bgRectTransform.DOKill();
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(canvasGroup.DOFade(1, 0.3f));
            sequence.Join(bgRectTransform.DOScale(Vector3.one, 0.3f)).SetEase(Ease.OutBack);

            sequence.AppendInterval(2f);
            
            sequence.Append(canvasGroup.DOFade(0, 0.3f));
            sequence.Join(bgRectTransform.DOMoveY(bgRectTransform.position.y + 100f, 0.3f));
            
            sequence.OnComplete(() => gameObject.SetActive(false));
        }
    }
}