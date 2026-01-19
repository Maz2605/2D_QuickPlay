using System;
using _Game.Core.Scripts.Audio.Manager;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.UI;
using _Game.Core.Scripts.UI.Base;
using UnityEngine;

namespace _Game.Core.Scripts.Audio
{
    [RequireComponent(typeof(BasePopup))]
    public class UIPopupSound : MonoBehaviour
    {
        [SerializeField] private UIAudioConfigSO config;

        [Header("Open Sound")] 
        [SerializeField] private UISoundType openType = UISoundType.PopupOpenStandard;
        [SerializeField] private AudioClip customOpenClip;
        
        [Header("Close Sound")]
        [SerializeField] private UISoundType closeType = UISoundType.PopupCloseStandard;
        [SerializeField] private AudioClip customCloseClip;
        
        private BasePopup _popup;

        private void Awake()
        {
            _popup = GetComponent<BasePopup>();
        }

        private void OnEnable()
        {
            if(_popup.OnOpen != null) _popup.OnOpen.AddListener(PlayOpenSound);
            if(_popup.OnClose != null) _popup.OnClose.AddListener(PlayCloseSound);
        }

        private void OnDisable()
        {
            if(_popup.OnOpen != null) _popup.OnOpen.RemoveListener(PlayOpenSound);
            if(_popup.OnClose != null) _popup.OnClose.RemoveListener(PlayCloseSound);
        }

        private void PlayOpenSound() => PlaySound(openType, customOpenClip);
        private void PlayCloseSound() => PlaySound(closeType, customCloseClip);
        
        private void PlaySound(UISoundType type, AudioClip customClip)
        {
            if (AudioManager.Instance == null) return;

            AudioClip clipToPlay = customClip;
            
            if (clipToPlay == null && config != null)
            {
                clipToPlay = config.GetClip(type);
            }

            if (clipToPlay != null)
            {
                float vol = config ? config.uiVolume : 1f;
                AudioManager.Instance.PlaySfx(clipToPlay, vol);
            }
        }
    }
}