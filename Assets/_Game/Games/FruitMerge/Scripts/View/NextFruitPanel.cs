using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.FruitMerge.Scripts.Config;

namespace _Game.Games.FruitMerge.Scripts.View
{
    public class NextFruitPanel : MonoBehaviour
    {
        [SerializeField] private Image[] slots;
        
        [Header("Animation")]
        [SerializeField] private float punchStrength = 0.3f;
        [SerializeField] private float duration = 0.2f;

        private void OnEnable()
        {
            EventManager<FruitMergeEventID>.AddListener<List<Sprite>>(
                FruitMergeEventID.NextFruitChanged, OnNextFruitChanged);
        }

        private void OnDisable()
        {
            EventManager<FruitMergeEventID>.RemoveListener<List<Sprite>>(
                FruitMergeEventID.NextFruitChanged, OnNextFruitChanged);
            
            transform.DOKill();
            foreach (var slot in slots) slot.transform.DOKill();
        }

        public void ResetPanel()
        {
            foreach (var slot in slots)
            {
                slot.transform.DOKill(); 
                slot.transform.localScale = Vector3.one;
                slot.gameObject.SetActive(false); 
            }
        }

        private void OnNextFruitChanged(List<Sprite> sprites)
        {
            if (sprites == null) return;

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].transform.DOKill();
                slots[i].transform.localScale = Vector3.one;

                if (i < sprites.Count)
                {
                    slots[i].sprite = sprites[i];
                    slots[i].gameObject.SetActive(true);
                    
                    slots[i].transform.DOPunchScale(Vector3.one * punchStrength, duration, 5, 1)
                        .SetDelay(i * 0.05f)
                        .SetLink(slots[i].gameObject);
                }
                else
                {
                    slots[i].gameObject.SetActive(false);
                }
            }
        }
    }
}