using System;
using UnityEngine;

namespace _Game.Core.Scripts.Data
{
    [Serializable]
    public class BaseGameData 
    {
        public string GameId;
        public int HightScore;
        public long LastPlayedTime;

        public BaseGameData()
        {
            HightScore = 0;
            LastPlayedTime = DateTime.Now.Ticks;
        }
    }
}
