using UnityEngine;
using DG.Tweening;
using _Game.Core.Scripts.Audio.Manager;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.WaterSort.Scripts.Config;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public class WaterSortAudioController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private WaterSortAudioConfig config;

        private AudioSource _pouringSource;
        private AudioManager Audio => AudioManager.Instance;

        private void Awake()
        {
            _pouringSource = gameObject.AddComponent<AudioSource>();
            if (config)
            {
                _pouringSource.clip = config.sfxPouringLoop;
            }
            _pouringSource.loop = true;
            _pouringSource.playOnAwake = false;
            _pouringSource.volume = 0;
        }

        private void OnEnable()
        {
            EventManager<WaterSortEventID>.AddListener<WaterSortState>(WaterSortEventID.GameStateChanged, OnGameStateChanged);
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.BottleSelected, PlaySelect);
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.BottleDeselected, PlaySelect);
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.MoveInvalid, PlayError);
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.UndoExecuted, PlayUndo);
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.LevelWin, PlayWin);
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.BottleSolved, PlayBottleSolved);
            
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.PouringStarted, OnPouringStart);
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.PouringCompleted, OnPouringStop);
        }

        private void OnDisable()
        {
            EventManager<WaterSortEventID>.RemoveListener<WaterSortState>(WaterSortEventID.GameStateChanged, OnGameStateChanged);

            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.BottleSelected, PlaySelect);
            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.BottleDeselected, PlaySelect);
            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.MoveInvalid, PlayError);
            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.UndoExecuted, PlayUndo);
            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.LevelWin, PlayWin);
            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.BottleSolved, PlayBottleSolved);
            
            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.PouringStarted, OnPouringStart);
            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.PouringCompleted, OnPouringStop);
            
            if (_pouringSource != null) _pouringSource.DOKill();
        }

        // --- HANDLERS ---

        private void OnGameStateChanged(WaterSortState state)
        {
            if (Audio == null) return;

            switch (state)
            {
                case WaterSortState.Intro:
                case WaterSortState.Idle:
                    Audio.PlayMusic(config.backgroundMusic, true, 1f);
                    break;
                
                case WaterSortState.Victory:
                    Audio.StopMusic();
                    break;
            }
        }

        private void PlaySelect() => TryPlaySfx(config.sfxSelect);
        private void PlayError() => TryPlaySfx(config.sfxError);
        private void PlayUndo() => TryPlaySfx(config.sfxUndo);
        private void PlayWin() => TryPlaySfx(config.sfxWin);
        private void PlayBottleSolved() => TryPlaySfx(config.sfxBottleComplete);

        private void OnPouringStart() => HandlePouringSound(true);
        private void OnPouringStop() => HandlePouringSound(false);

        // --- HELPER METHODS ---

        private void TryPlaySfx(AudioClip clip)
        {
            if (Audio != null && clip != null) 
                Audio.PlaySfx(clip, config.sfxVolume);
        }

        private void HandlePouringSound(bool isPouring)
        {
            if (Audio == null || !Audio.IsSfxEnabled || _pouringSource == null) return;

            _pouringSource.DOKill(); 

            if (isPouring)
            {
                _pouringSource.volume = 0;
                _pouringSource.Play();
                _pouringSource.DOFade(config.sfxVolume, 0.2f).SetTarget(_pouringSource);
            }
            else
            {
                _pouringSource.DOFade(0f, 0.15f)
                    .SetTarget(_pouringSource)
                    .OnComplete(() => 
                    {
                        if(_pouringSource != null) _pouringSource.Stop();
                    });
            }
        }
    }
}