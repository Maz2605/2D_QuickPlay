using System;
using _Game.Core.Scripts.Data;

namespace _Game.Games.FruitMerge.Scripts.Model
{
    [Serializable]
    public class FruitMergeUserData : IGameUserData
    {
        public int HighScore { get; set; }
    }
}