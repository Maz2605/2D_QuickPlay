using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.Config
{
    [CreateAssetMenu(fileName = "Audio Config", menuName = "Games/FruitMerge/Audio Config")]
    public class FruitAudioConfigSO: ScriptableObject
    {
        [Header("Music")] 
        public AudioClip backgroundMusic;

        [Header("SFX")] 
        public AudioClip dropSound;
        public AudioClip mergeSound;
        public AudioClip bigMergeSound;
        public AudioClip comboSound;
        
        
        [Header("Volume Settings")]
        [Range(0f, 1f)] public float dropVolume = 0.6f;      // Thay cho số 0.6f cứng
        [Range(0f, 1f)] public float normalMergeVol = 0.7f;  // Thay cho số 0.7f cứng
        [Range(0f, 1f)] public float bigMergeVol = 1.0f;
        
        [Header("Logic Settings")]
        [Range(0f, 0.5f)] public float pitchVariation = 0.1f;
        public int bigMergeThreshold = 8;
    }
}