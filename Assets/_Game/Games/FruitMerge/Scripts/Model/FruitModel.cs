using System;
using UnityEngine.Serialization;

namespace _Game.Games.FruitMerge.Scripts.Model
{
    [Serializable]
    public class FruitModel
    {
        [FormerlySerializedAs("Level")] public int level;         
        [FormerlySerializedAs("InstanceID")] public int instanceID;     
        [FormerlySerializedAs("IsMerging")] public bool isMerging;     

        public FruitModel(int level, int instanceID)
        {
            this.level = level;
            this.instanceID = instanceID;
            isMerging = false;
        }
    }
}