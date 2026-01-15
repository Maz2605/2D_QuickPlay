using System.Collections;
using System.Collections.Generic;
using _Game.Core.Scripts.Utils.DesignPattern.Singleton;
using DG.Tweening;
using UnityEngine;

namespace _Game.Core.Scripts.Audio.Manager
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Setup")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSourcePrefab;
        [SerializeField] private int sfxPoolSize = 15;
        
        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;
        
        private bool _isSfxEnabled = false;

        private Queue<AudioSource> _sfxPool;

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
        }

        private void InitializePool()
        {
            _sfxPool = new Queue<AudioSource>();
            GameObject poolRoot = new GameObject("SFX_Pool");
            poolRoot.transform.SetParent(transform);

            for (int i = 0; i < sfxPoolSize; i++)
            {
                var audioSource = Instantiate(sfxSourcePrefab, poolRoot.transform);
                audioSource.gameObject.SetActive(false);
                _sfxPool.Enqueue(audioSource);
            }
        }
        
        //===Public API ===
        
        private void UpdateMusicVolume()
        {
            if (musicSource != null) 
                musicSource.volume = _musicVolume * _masterVolume;
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateMusicVolume();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            UpdateMusicVolume();
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
        }

        public void SetSfxState(bool state)
        {
            _isSfxEnabled = state;
        }
        
        public void PlayMusic(AudioClip clip, bool loop = true, float fadeTime = 0.5f)
        {
            if(clip == null) return;
            if(musicSource.clip == clip) return;

            float targetVol = _musicVolume * _masterVolume;

            musicSource.DOFade(0, fadeTime / 2).OnComplete(() =>
            {
                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.Play();
                musicSource.DOFade(targetVol, fadeTime / 2);
            });
        }
        
        public void PlaySfx(AudioClip clip, float volScale = 1f, float pitchVar = 0f)
        {
            if (clip == null) return;

            AudioSource source = GetSfxSource();
            source.clip = clip;
            
            source.volume = _sfxVolume * _masterVolume * volScale;
            
            if (pitchVar > 0) source.pitch = 1f + Random.Range(-pitchVar, pitchVar);
            else source.pitch = 1f;

            source.gameObject.SetActive(true);
            source.Play();

            StartCoroutine(ReturnToPool(source, clip.length));
        }
        
        public void StopMusic() => musicSource.Stop();
        
        //===Pool Helper (Giữ nguyên)===
        private AudioSource GetSfxSource()
        {
            if (_sfxPool.Count == 0) return Instantiate(sfxSourcePrefab, transform);
            return _sfxPool.Dequeue();
        }

        private IEnumerator ReturnToPool(AudioSource source, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            source.Stop();
            source.gameObject.SetActive(false);
            _sfxPool.Enqueue(source);
        }
    }
}