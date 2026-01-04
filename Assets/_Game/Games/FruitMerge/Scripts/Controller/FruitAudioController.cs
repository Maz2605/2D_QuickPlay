using System;
using _Game.Core.Scripts.Input;
using _Game.Core.Scripts.Manager;
using _Game.Games.FruitMerge.Scripts.Config;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public class FruitAudioController : MonoBehaviour
    {
        [SerializeField] private FruitAudioConfigSO config;
        
        private AudioManager Audio => AudioManager.Instance;

        public void Initialize(FruitGameEntryPoint entryPoint)
        {
            Audio.PlayMusic(config.backgroundMusic);

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTouchEnd += PlayDrop;
            }

            entryPoint.OnFruitMerged += PlayMerge;
        }

        private void OnDestroy()
        { 
            if(InputManager.Instance != null) InputManager.Instance.OnTouchEnd -= PlayDrop;
            if(Audio != null) Audio.StopMusic();
        }

        private void PlayDrop(Vector2 position)
        {
            Audio.PlaySfx(config.dropSound, config.dropVolume, config.pitchVariation);
        }

        private void PlayMerge(int newLevel)
        {
            if (newLevel >= 8)
            {
                Audio.PlaySfx(config.bigMergeSound, config.bigMergeVol, 0f);
            }
            else
            {
                Audio.PlaySfx(config.mergeSound, config.normalMergeVol, config.pitchVariation);
            }
        }
        
    }
}