using _Game.Core.Scripts.Audio;
using UnityEngine;

namespace _Game.Games.WaterSort.Scripts.Config
{
    [CreateAssetMenu(fileName = "WaterSortAudioConfig", menuName = "Games/WaterSort/Audio Config")]
    public class WaterSortAudioConfig : BaseAudioConfigSO
    {
        [Header("--- SFX ---")]
        public AudioClip sfxSelect;       
        public AudioClip sfxPouringLoop;  
        public AudioClip sfxWin;          
        public AudioClip sfxError;        
        public AudioClip sfxUndo;
        public AudioClip sfxBottleComplete;

        [Header("--- VOLUMES ---")]
        [Range(0f, 1f)] public float musicVolume = 0.5f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 0.5f)] public float pitchVariation = 0.1f;
    }
}