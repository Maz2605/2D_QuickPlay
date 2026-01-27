using _Game.Core.Scripts.Audio;
using UnityEngine;

namespace _Game.Games.BlockSlide.Scripts.Config
{
    [CreateAssetMenu(fileName = "BlockSlideAudioConfig", menuName = "Games/BlockSlide/AudioConfig")]
    public class BlockSlideAudioConfig : BaseAudioConfigSO
    {
        [Header("Sound Effects")] 
        public AudioClip sfxMove;
        public AudioClip sfxMerge;
        public AudioClip sfxGameOver;
        
    }
}