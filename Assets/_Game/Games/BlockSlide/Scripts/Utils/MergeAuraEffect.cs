using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Games.BlockSlide.Scripts.Utils
{
    public class MergeAuraEffect : MonoBehaviour
    {
        [SerializeField] private Image auraImage;
        [SerializeField] private float duration = 0.4f;
        [SerializeField] private float endScale = 2.5f;

        public void Play(Vector3 startPosition, Color auraColor)
        {
            transform.position = startPosition;
            transform.localScale = Vector3.one; 

            auraColor.a = 1f; 
            auraImage.color = auraColor; 
        
            transform.DOKill();
            auraImage.DOKill();

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(Vector3.one * endScale, duration).SetEase(Ease.OutQuad));
            seq.Join(auraImage.DOFade(0f, duration).SetEase(Ease.InQuad));

            seq.OnComplete(() =>
            {
                PoolingManager.Instance.Despawn(this.gameObject);
            });
        }
    }
}