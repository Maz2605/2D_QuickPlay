using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Gọi DOTween

namespace _Game.Games.FruitMerge.Scripts.View
{
    public class NextFruitPanel : MonoBehaviour
    {
        [SerializeField] private Image[] slots;
        
        // Hiệu ứng
        [Header("Animation")]
        [SerializeField] private float punchStrength = 0.3f;
        [SerializeField] private float duration = 0.2f;

        public void UpdatePreview(List<Sprite> sprites)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].transform.DOKill();
                slots[i].transform.localScale = Vector3.one;

                if (i < sprites.Count)
                {
                    slots[i].sprite = sprites[i];
                    slots[i].gameObject.SetActive(true);
                    slots[i].transform.DOPunchScale(Vector3.one * punchStrength, duration, 5, 1)
                        .SetDelay(i * 0.05f);
                }
                else
                {
                    slots[i].gameObject.SetActive(false);
                }
            }
        }
        
        private void OnDestroy()
        {
            foreach (var slot in slots) slot.transform.DOKill();
        }
    }
}