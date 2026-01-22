using UnityEngine;
using _Game.Core.Scripts.Audio.Manager;
using _Game.Games.WaterSort.Scripts.Config;
using DG.Tweening;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public class WaterSortAudioController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private WaterSortAudioConfig config;
        [SerializeField] private WaterSortController gameController;

        private AudioSource _pouringSource;
        
        private AudioManager _audioManager; 

        private void Start()
        {
            if (config == null || gameController == null) return;
            
            _audioManager = AudioManager.Instance;

            _pouringSource = gameObject.AddComponent<AudioSource>();
            _pouringSource.clip = config.sfxPouringLoop;
            _pouringSource.loop = true;
            _pouringSource.playOnAwake = false;
            _pouringSource.volume = 0;

            if (_audioManager != null)
            {
                _audioManager.PlayMusic(config.backgroundMusic, true, 1f);
            }
            
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();

            if (_pouringSource != null)
            {
                _pouringSource.DOKill(); 
            }

            if (_audioManager != null) 
            {
                _audioManager.StopMusic();
            }
        }

        private void RegisterEvents()
        {
            if (gameController == null) return;
            gameController.OnBottleSelected += PlaySelect;
            gameController.OnBottleDeselected += PlaySelect;
            gameController.OnMoveInvalid += PlayError;
            gameController.OnUndo += PlayUndo;
            gameController.OnLevelWin += PlayWin;
            gameController.OnPouringStateChanged += HandlePouringSound;
            gameController.OnBottleSolved += PlayBottleSolved;
        }

        private void UnregisterEvents()
        {
            if (gameController == null) return;
            gameController.OnBottleSelected -= PlaySelect;
            gameController.OnBottleDeselected -= PlaySelect;
            gameController.OnMoveInvalid -= PlayError;
            gameController.OnUndo -= PlayUndo;
            gameController.OnLevelWin -= PlayWin;
            gameController.OnPouringStateChanged -= HandlePouringSound;
            gameController.OnBottleSolved -= PlayBottleSolved;
        }

        private AudioManager Audio => _audioManager; 

        private void PlaySelect() => TryPlaySfx(config.sfxSelect);
        private void PlayError() => TryPlaySfx(config.sfxError);
        private void PlayUndo() => TryPlaySfx(config.sfxUndo);
        private void PlayWin() => TryPlaySfx(config.sfxWin);
        private void PlayBottleSolved() => TryPlaySfx(config.sfxBottleComplete);

        private void TryPlaySfx(AudioClip clip)
        {
            if (Audio != null) Audio.PlaySfx(clip, config.sfxVolume);
        }

        private void HandlePouringSound(bool isPouring)
        {
            if (Audio == null || !Audio.IsSfxEnabled || _pouringSource == null) return;

            if (isPouring)
            {
                _pouringSource.volume = 0;
                _pouringSource.Play();
                _pouringSource.DOFade(config.sfxVolume, 0.2f).SetTarget(_pouringSource); // SetTarget giúp DOKill hoạt động chính xác
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