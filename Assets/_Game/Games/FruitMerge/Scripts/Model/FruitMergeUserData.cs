using System;
using System.Collections.Generic;
using _Game.Core.Scripts.Data;
using UnityEngine.Serialization;

namespace _Game.Games.FruitMerge.Scripts.Model
{
    [Serializable]
    public class FruitMergeUserData : IGameUserData
    {
        public int HighScore { get; set; }

        public List<int> topScores;

        public FruitMergeUserData()
        {
            topScores = new List<int>() {0, 0, 0};
        }
    }
}