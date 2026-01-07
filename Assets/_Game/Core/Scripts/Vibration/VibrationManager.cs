using _Game.Core.Scripts.Utils.DesignPattern.Singleton;
using UnityEngine;

namespace _Game.Core.Scripts.Vibration
{
    public class VibrationManager : Singleton<VibrationManager>
    {
        public bool IsVibrationsEnabled { get; private set; } = true;

        protected override void Awake()
        {
            base.Awake();
            
            IsVibrationsEnabled = true;
        }

        public void ToggleVibration(bool isOn)
        {
            IsVibrationsEnabled = isOn;
            
        }
        public void HapticLight()
        {
            if(!IsVibrationsEnabled) return;
            
            Handheld.Vibrate();
        }
    }
}