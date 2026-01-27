using _Game.Core.Scripts.Audio.Manager;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using UnityEngine;

namespace _Game.Games.BlockSlide.Scripts.Controller
{
    public class BlockSlideAudioController : MonoBehaviour
    {
        [SerializeField] private BlockSlideAudioConfig config;

        private int _lastScore;
        
        private void Start()
        {
            _lastScore = 0;
        }

        private void OnEnable()
        {
            EventManager<BlockSlideEventID>.AddListener<BlockSlideState>(BlockSlideEventID.GameStateChanged, OnBlockSlideState);
            EventManager<BlockSlideEventID>.AddListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
            EventManager<BlockSlideEventID>.AddListener(BlockSlideEventID.BoardUpdate, OnBlockMove);
        }

        private void OnDisable()
        {
            EventManager<BlockSlideEventID>.RemoveListener<BlockSlideState>(BlockSlideEventID.GameStateChanged, OnBlockSlideState);
            EventManager<BlockSlideEventID>.RemoveListener<int>(BlockSlideEventID.ScoreUpdate, OnScoreUpdate);
            EventManager<BlockSlideEventID>.RemoveListener(BlockSlideEventID.BoardUpdate, OnBlockMove);
        }

        private void OnBlockMove()
        {
            AudioManager.Instance.PlaySfx(config.sfxMove);
        }

        private void OnScoreUpdate(int currentScore)
        {
            if (currentScore > _lastScore)
            {
                AudioManager.Instance.PlaySfx(config.sfxMerge);
            }
            _lastScore = currentScore;
        }

        private void OnBlockSlideState(BlockSlideState state)
        {
            switch (state)
            {
                case BlockSlideState.Playing:
                    AudioManager.Instance.PlayMusic(config.backgroundMusic);
                    break;
                case BlockSlideState.GameOver:
                    AudioManager.Instance.StopMusic();
                    AudioManager.Instance.PlaySfx(config.sfxGameOver);
                    break;
                default:
                    AudioManager.Instance.StopMusic();
                    break;
            }
        }
    }
}