using _Game.Core.Scripts.Audio;
using _Game.Core.Scripts.Audio.Manager;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.Utils.DesignPattern.Singleton;
using _Game.Core.Scripts.Vibration;

namespace _Game.Core.Scripts.GameSystem
{
    public class SettingsManager : Singleton<SettingsManager>
    {
        private const string SAVE_ID = "global_user_settings";
        public GlobalUserSetting CurrentSettings { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            LoadData();
        }

        private void Start()
        {
            ApplyAllSettings();
        }

        private void LoadData()
        {
            CurrentSettings = SaveSystem.Load<GlobalUserSetting>(SAVE_ID);
            
            if (CurrentSettings == null)
            {
                CurrentSettings = new GlobalUserSetting();
            }
        }

        private void ApplyAllSettings()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(CurrentSettings.masterVolume);
                AudioManager.Instance.SetMusicVolume(CurrentSettings.musicVolume);
                AudioManager.Instance.SetSfxVolume(CurrentSettings.sfxVolume);
                AudioManager.Instance.SetSfxState(CurrentSettings.isSfxEnabled);
            }

            if (VibrationManager.Instance != null)
            {
                VibrationManager.Instance.ToggleVibration(CurrentSettings.isVibrationEnabled);
            }
        }

        private void SaveData()
        {
            SaveSystem.Save(SAVE_ID, CurrentSettings);
        }

        // PUBLIC API (Gắn vào UI Slider / Toggle)
        public void SetVibrationState(bool isOn)
        {
            CurrentSettings.isVibrationEnabled = isOn;
            
            if (VibrationManager.Instance != null)
                VibrationManager.Instance.ToggleVibration(isOn);
            SaveData();
        }

        public void SetMasterVolume(float value)
        {
            CurrentSettings.masterVolume = value;
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMasterVolume(value);
                
            SaveData();
        }

        public void SetMusicVolume(float value)
        {
            CurrentSettings.musicVolume = value;
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMusicVolume(value);
                
            SaveData();
        }

        public void SetSfxVolume(float value)
        {
            CurrentSettings.sfxVolume = value;
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetSfxVolume(value);
                
            SaveData();
        }
        
        public void SetSfxState(bool isOn)
        {
            CurrentSettings.isSfxEnabled = isOn;
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetSfxState(isOn);
            SaveData();
        }
    }
    
}