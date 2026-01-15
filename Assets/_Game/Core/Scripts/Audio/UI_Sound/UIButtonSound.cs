using System;
using _Game.Core.Scripts.Audio.Manager;
using _Game.Core.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Core.Scripts.Audio
{
    [RequireComponent(typeof(Button))]
    public class UIButtonSound : MonoBehaviour
    {
        [Header("Config References")]
        [SerializeField] private UIAudioConfigSO config;
        
        [Header("Sound Settings")]
        [SerializeField] private UISoundType soundType = UISoundType.ClickNormal;
        [SerializeField] private AudioClip customClipOverride;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(PlaySound);
        }

        private void PlaySound()
        {
            if(AudioManager.Instance == null) return;
            
            AudioClip clipToPlay = null;
            float volume = 1f;
            
            if (customClipOverride != null)
            {
                clipToPlay = customClipOverride;
                volume = config != null ? config.uiVolume : 1f;
            }
            else if (config != null)
            {
                clipToPlay = config.GetClip(soundType);
                volume = config.uiVolume;
            }

            if (clipToPlay != null)
            {
                AudioManager.Instance.PlaySfx(clipToPlay, volume, 0f);
            }
        }
        
    }
}