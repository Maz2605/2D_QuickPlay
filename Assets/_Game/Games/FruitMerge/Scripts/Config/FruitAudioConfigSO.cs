using _Game.Core.Scripts.Audio;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.Config
{
    [CreateAssetMenu(fileName = "Audio Config", menuName = "Games/FruitMerge/Audio Config")]
    public class FruitAudioConfigSO: BaseAudioConfigSO
    {
        [Header("SFX")] 
        public AudioClip dropSound;
        public AudioClip mergeSound;
        public AudioClip bigMergeSound;
        public AudioClip comboSound;
        public AudioClip gameOverSound;
    }
}