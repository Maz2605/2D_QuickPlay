using System.Collections;
using System.Collections.Generic;
using _Game.Core.Scripts.Data;
using _Script.DesignPattern.Singleton;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace _Game.Core.Scripts.Manager
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Setup")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSourcePrefab;
        [SerializeField] private int sfxPoolSize = 15;
        
        //Data Runtime
        private GlobalUserSetting _setting;
        private const string SETTING_FILE_NAME = "globalSettings";
        private Queue<AudioSource> _sfxPool;
        
        //Public Properties (Read_Only)
        public float MasterVolume => _setting.masterVolume;
        public float MusicVolume => _setting.musicVolume;
        public float SfxVolume => _setting.sfxVolume;

        protected override void Awake()
        {
            base.Awake();
            LoadSettings();
            InitializePool();
        }

        private void LoadSettings()
        {
            _setting = SaveSystem.Load<GlobalUserSetting>(SETTING_FILE_NAME);
            if(_setting == null)
                _setting = new GlobalUserSetting();
            
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
        
        //===Public API Setting===
        
        private void UpdateMusicVolume()
        {
            if (musicSource != null) musicSource.volume = MusicVolume * MasterVolume;
        }

        public void SetMasterVolume(float volume)
        {
            _setting.masterVolume = Mathf.Clamp01(volume);
            UpdateMusicVolume();
            SaveSystem.Save(SETTING_FILE_NAME, _setting);
        }

        public void SetMusicVolume(float volume)
        {
            _setting.musicVolume = Mathf.Clamp01(volume);
            UpdateMusicVolume();
            SaveSystem.Save(SETTING_FILE_NAME, _setting);
        }

        public void SetSfxVolume(float volume)
        {
            _setting.sfxVolume = Mathf.Clamp01(volume);
            SaveSystem.Save(SETTING_FILE_NAME, _setting);
        }
        
        //===Playback Logic===
        public void PlayMusic(AudioClip clip, bool loop = false, float fadeTime = 0.5f)
        {
            if(clip == null) return;
            if(musicSource.clip == clip) return;

            musicSource.DOFade(0, fadeTime / 2).OnComplete(() =>
            {
                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.Play();
                musicSource.DOFade(MusicVolume * MasterVolume, fadeTime / 2);
            });
        }
        
        public void PlaySfx(AudioClip clip, float volScale = 1f, float pitchVar = 0f)
        {
            if (clip == null) return;

            AudioSource source = GetSfxSource();
            source.clip = clip;
            source.volume = SfxVolume * MasterVolume * volScale;
            
            if (pitchVar > 0) source.pitch = 1f + Random.Range(-pitchVar, pitchVar);
            else source.pitch = 1f;

            source.gameObject.SetActive(true);
            source.Play();

            StartCoroutine(ReturnToPool(source, clip.length));
        }
        
        public void StopMusic() => musicSource.Stop();
        //===Pool Helper==
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