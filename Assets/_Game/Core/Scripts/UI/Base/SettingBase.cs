using _Game.Core.Scripts.GameSystem;
using UnityEngine;
using UnityEngine.UI;
using _Game.Core.Scripts.GameSystem; 
using _Game.Core.Scripts.UI;
using UnityEngine.Serialization;

namespace _Game.Core.Scripts.UI
{
    public abstract class SettingBase : BasePopup 
    {
        [Header("--- Data Binding (Logic) ---")]
        [SerializeField] protected Slider masterSlider;
        [SerializeField] protected Slider musicSlider;
        [SerializeField] protected SwitchToggle sfxToggle;
        [SerializeField] protected SwitchToggle vibrationToggle;
        
        [Header("--- Buttons ---")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button backgroundButton;

        protected override void Awake()
        {
            base.Awake(); 
            
            if (closeButton) closeButton.onClick.AddListener(OnCloseClicked);
            if (backgroundButton) backgroundButton.onClick.AddListener(OnCloseClicked);
        }

        public override void Show()
        {
            base.Show(); 
            InitData(); 
        }

        private void InitData()
        {
            var settings = SettingsManager.Instance.CurrentSettings;

            // Helper function setup slider
            void Bind(Slider s, float val, UnityEngine.Events.UnityAction<float> act)
            {
                if (s == null) return;
                s.SetValueWithoutNotify(val);
                s.onValueChanged.RemoveAllListeners();
                s.onValueChanged.AddListener(act);
            }

            Bind(masterSlider, settings.masterVolume, SettingsManager.Instance.SetMasterVolume);
            Bind(musicSlider, settings.musicVolume, SettingsManager.Instance.SetMusicVolume);

            if (sfxToggle)
            {
                sfxToggle.onValueChanged.RemoveAllListeners();
                sfxToggle.ForceSetState(settings.isSfxEnabled);
                sfxToggle.onValueChanged.AddListener(SettingsManager.Instance.SetSfxState);
            }
            if (vibrationToggle)
            {
                vibrationToggle.onValueChanged.RemoveAllListeners();
                vibrationToggle.ForceSetState(settings.isVibrationEnabled);
                vibrationToggle.onValueChanged.AddListener(SettingsManager.Instance.SetVibrationState);
            }
        }

        protected void OnCloseClicked()
        {
            Hide();
        }
    }
}