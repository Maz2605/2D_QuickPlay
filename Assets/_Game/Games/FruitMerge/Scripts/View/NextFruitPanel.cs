using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Games.FruitMerge.Scripts.View
{
    public class NextFruitPanel : MonoBehaviour
    {
        [SerializeField] private Image[] slots;

        public void UpdatePreview(List<Sprite> sprites)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < sprites.Count)
                {
                    slots[i].sprite = sprites[i];
                    slots[i].gameObject.SetActive(true);
                }
                else
                {
                    slots[i].gameObject.SetActive(false);
                }
            }
        }
    }
}