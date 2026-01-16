using _Game.Core.Scripts.Audio.Manager;
using _Game.Core.Scripts.Data;
using UnityEngine;

namespace _Game.Core.Scripts.Audio.UI_Sound
{
    public class UIEventSound : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private UISoundType openSound = UISoundType.None;
        [SerializeField] private UISoundType closeSound = UISoundType.None;

        [Header("Custom (Optional)")]
        [SerializeField] private AudioClip customOpenClip;
        [SerializeField] private AudioClip customCloseClip;

        private void OnEnable()
        {
            PlaySound(openSound, customOpenClip);
        }

        private void OnDisable()
        {
            if (gameObject.scene.isLoaded) 
            {
                PlaySound(closeSound, customCloseClip);
            }
        }

        private void PlaySound(UISoundType type, AudioClip customClip)
        {
            if (AudioManager.Instance == null) return;

            if (customClip != null)
            {
                AudioManager.Instance.PlaySfx(customClip);
                return;
            }

            if (type != UISoundType.None)
            {
                AudioManager.Instance.PlayUISound(type);
            }
        }
    }
}