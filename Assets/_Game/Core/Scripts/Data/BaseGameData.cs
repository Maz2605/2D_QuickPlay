using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Scripts.Data
{
    [Serializable]
    public class BaseGameData 
    {
        [FormerlySerializedAs("GameId")] public string gameId;
        [FormerlySerializedAs("HightScore")] public int hightScore;
        [FormerlySerializedAs("LastPlayedTime")] public long lastPlayedTime;

        public BaseGameData()
        {
            hightScore = 0;
            lastPlayedTime = DateTime.Now.Ticks;
        }
    }
}
