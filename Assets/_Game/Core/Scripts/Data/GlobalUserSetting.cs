using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Scripts.Data
{
    [Serializable]
    public class GlobalUserSetting 
    {
        public float masterVolume = 1f;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        
        public bool isSfxEnabled = true;
        public bool isVibrationEnabled = true;
    }
}
