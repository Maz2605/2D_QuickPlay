using System;
using System.Collections.Generic;
using _Game.Core.Scripts.Data;

namespace _Game.Games.BlockSlide.Scripts.Model
{
    [Serializable]
    public class BlockSlideUserData : IGameUserData
    {
        public List<int> topScores = new List<int>();

        public BlockSlideUserData()
        {
            topScores = new List<int> { 0, 0, 0 };
        }

        public int HighScore { get; set; }
    }
}