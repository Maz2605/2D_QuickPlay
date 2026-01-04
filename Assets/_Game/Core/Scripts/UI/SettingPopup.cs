using _Game.Core.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Core.Scripts.UI
{
    public class SettingPopup : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            // Init giá trị từ AudioManager
            var audio = AudioManager.Instance;
            if (audio == null) return;

            masterSlider.value = audio.MasterVolume;
            musicSlider.value = audio.MusicVolume;
            sfxSlider.value = audio.SfxVolume;

            // Đăng ký sự kiện
            masterSlider.onValueChanged.AddListener(audio.SetMasterVolume);
            musicSlider.onValueChanged.AddListener(audio.SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(audio.SetSfxVolume);
        }
    }
}