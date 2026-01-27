using _Game.Core.Scripts.Data;
using UnityEngine;

namespace _Game.Games.BlockSlide.Scripts.Config
{
    [CreateAssetMenu(fileName = "BlockSlideConfigSO", menuName = "Games/BlockSlide/GameConfig")]
    public class BlockSlideConfigSO : ScriptableObject
    {
        [Header("Block Slide Config")]
        public GameProfileSO gameProfile;
        public int boardWith = 4;
        public int boardHeight = 4;
    }
}