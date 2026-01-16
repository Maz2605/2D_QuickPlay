using _Game.Core.Scripts.Audio.Manager;
using _Game.Core.Scripts.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Core.Scripts.Audio.UI_Sound
{
    public class UIButtonSound : MonoBehaviour, IPointerClickHandler
    {
        // Biến public để Editor truy cập dễ dàng
        public UISoundType soundType = UISoundType.ClickNormal;
        
        // Chỉ dùng khi type = Custom
        public AudioClip customClip; 
        [Range(0f, 1f)] public float volumeScale = 1f;

        public void OnPointerClick(PointerEventData eventData)
        {
            PlaySound();
        }

        public void PlaySound()
        {
            if (AudioManager.Instance == null) return;

            if (soundType == UISoundType.Custom)
            {
                // Logic Custom: Play file riêng
                if (customClip != null) 
                    AudioManager.Instance.PlaySfx(customClip, volumeScale);
            }
            else
            {
                // Logic Standard: Gọi qua Enum
                AudioManager.Instance.PlayUISound(soundType);
            }
        }
    }
}