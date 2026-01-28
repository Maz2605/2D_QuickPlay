using _Game.Core.Scripts.Audio.Manager;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.FruitMerge.Scripts.Config;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public class FruitAudioController : MonoBehaviour
    {
        [SerializeField] private FruitAudioConfigSO config;
        
        private AudioManager Audio => AudioManager.Instance;
        

        private void OnEnable()
        {
            EventManager<FruitMergeEventID>.AddListener<FruitMergeState>(FruitMergeEventID.GameStateChanged, OnGameStateChanged);
            EventManager<FruitMergeEventID>.AddListener(FruitMergeEventID.FruitDropped, OnFruitDropped);
            EventManager<FruitMergeEventID>.AddListener<int>(FruitMergeEventID.FruitMerged, OnMerged);
        }

        private void OnGameStateChanged(FruitMergeState state)
        {
            switch (state)
            {
                case FruitMergeState.Playing:
                    Audio.PlayMusic(config.backgroundMusic);
                    break;
                case FruitMergeState.GameOver:
                    Audio.StopMusic();
                    Audio.PlaySfx(config.gameOverSound);
                    break;
                default:
                    Audio.StopMusic();
                    break;
            }
        }

        private void OnDisable()
        {
            EventManager<FruitMergeEventID>.RemoveListener(FruitMergeEventID.FruitDropped, OnFruitDropped);
        }
        

        private void OnFruitDropped()
        {
            Audio.PlaySfx(config.dropSound);
        }

        private void OnMerged(int newLevel)
        {
            if (newLevel >= 8)
            {
                Audio.PlaySfx(config.bigMergeSound);
            }
            else
            {
                Audio.PlaySfx(config.mergeSound);
            }
        }
        
    }
}