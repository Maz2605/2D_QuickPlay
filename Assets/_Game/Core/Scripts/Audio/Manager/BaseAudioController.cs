using System;
using UnityEngine;

namespace _Game.Core.Scripts.Audio.Manager
{
    public class BaseAudioController : MonoBehaviour
    {
        [SerializeField] BaseAudioConfigSO configSO;

        private AudioManager Audio => AudioManager.Instance;

        public void Initialize()
        {
            Audio.PlayMusic(configSO.backgroundMusic);
        }

        private void OnDestroy()
        {
            if(AudioManager.HasInstance) return;
            Audio.StopMusic();
        }
    }
}