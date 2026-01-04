using _Game.Core.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Core.Scripts.UI
{
    public class SettingPopup : BasePopup
    {
        [Header("Audio Sliders")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        
        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button backgroundButton; // Nút mờ mờ đằng sau để bấm ra ngoài là tắt

        private void Start()
        {
            // Setup nút đóng
            if (closeButton) closeButton.onClick.AddListener(OnCloseClicked);
            if (backgroundButton) backgroundButton.onClick.AddListener(OnCloseClicked);

            // Init giá trị Slider từ AudioManager
            var audio = AudioManager.Instance;
            if (audio != null)
            {
                masterSlider.value = audio.MasterVolume;
                musicSlider.value = audio.MasterVolume;
                sfxSlider.value = audio.SfxVolume;

                // Đăng ký sự kiện kéo slider
                masterSlider.onValueChanged.AddListener(audio.SetMasterVolume);
                musicSlider.onValueChanged.AddListener(audio.SetMusicVolume);
                sfxSlider.onValueChanged.AddListener(audio.SetSfxVolume);
            }
        }

        private void OnCloseClicked()
        {
            Hide(); // Gọi hàm Hide có hiệu ứng của BasePopup
            
            // Resume game (Nếu logic game của bạn cần)
            // Time.timeScale = 1; 
            // Lưu ý: Việc Pause/Resume nên để UIManager điều phối thì tốt hơn
        }
    }
    
}