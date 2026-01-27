using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using _Game.Games.BlockSlide.Scripts.Config;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using _Game.Games.BlockSlide.Scripts.Utils;

namespace _Game.Games.BlockSlide.Scripts.View
{
    public class BlockView : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI numberText;

        [Header("VFX Prefabs")]
        [SerializeField] private MergeAuraEffect auraPrefab; 

        [Header("Juice Config")]
        [SerializeField] private float animDuration = 0.2f;       
        [SerializeField] private float blockPunchBase = 0.2f;     
        [SerializeField] private float textPunchBase = 0.5f;     

      public void SetData(int value, BlockColorConfigSO config, bool isSpawning, bool isMerging)
        {
            ResetState();

            BlockStyle style = null;
            if (value > 0)
            {
                style = config.GetStyle(value);
                numberText.text = value.ToString();
                numberText.color = style.textColor;
                backgroundImage.color = style.backgroundColor;
            }
            else
            {
                numberText.text = "";
                backgroundImage.color = config.defaultBackgroundColor;
                return; 
            }

            if (!isSpawning && !isMerging) return;

            if (style != null)
            {
                backgroundImage.color = Color.white;
                backgroundImage.DOColor(style.backgroundColor, animDuration).SetEase(Ease.OutQuad);
            }

            if (isSpawning)
            {
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one, animDuration).SetEase(Ease.OutBack);
            }
            else if (isMerging)
            {
                SpawnAura(style.backgroundColor);

                float randomFactor = Random.Range(0.8f, 1.3f);
                Vector3 finalBlockPunch = Vector3.one * blockPunchBase * randomFactor;
                Vector3 finalTextPunch = Vector3.one * textPunchBase * randomFactor;

                transform.DOPunchScale(finalBlockPunch, animDuration, 10, 1);
                
                numberText.transform.localScale = Vector3.one;
                numberText.transform.DOPunchScale(finalTextPunch, animDuration, 10, 1);
            }
        }

        private void SpawnAura(Color color)
        {
            if (auraPrefab == null) return;

            var auraObj = PoolingManager.Instance.Spawn(
                auraPrefab.gameObject,
                transform.position,
                Quaternion.identity,
                transform.parent 
            );

            var auraScript = auraObj.GetComponent<MergeAuraEffect>();
            if (auraScript != null)
            {
                auraScript.Play(transform.position, color);
                auraObj.transform.SetAsFirstSibling();
            }
        }

        public void ResetView()
        {
            ResetState();
            numberText.text = ""; 
        }

        private void ResetState()
        {
            transform.DOKill();
            backgroundImage.DOKill();
            numberText.transform.DOKill();

            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity; 
            
            numberText.transform.localScale = Vector3.one;
        }
    }
}