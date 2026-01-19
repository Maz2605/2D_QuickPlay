using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Game.Games.FruitMerge.Scripts.Config
{
    [CreateAssetMenu(fileName = "FruitGameConfig", menuName = "Games/FruitMerge/Game Config")]
    public class FruitMergeGameConfigSO : ScriptableObject
    {
        [Header("Game Settings")] public string gameID = "fruit_merge";
        public float spawnCooldown = 0.5f;

        [Header("Smart Logic")] public float rescueRate = 0.8f;

        [Header("Fruit Levels")] public FruitInfo[] fruits;

        [Serializable]
        public class FruitInfo
        {
            public string name;
            public Sprite visual;
            public int scoreValue;
            public float physicRadius;
            public float mass = 1f;
            public float scale = 1f;

            [Header("Spawn Rate")] public bool isSpawnable;
            [Range(0, 100)] public int spawnWeight;
        }

        //Logic Random
        [NonSerialized]private int _totalWeight = -1;
        [NonSerialized]private readonly List<int> _spawnableIndices = new List<int>();

        private void OnEnable()
        {
            _totalWeight = -1;
        }

        private void OnValidate()
        {
            _totalWeight = -1;
        }

        public int GetSmartSpawnLevel(bool isDanger, int lastLevel)
        {
            if (isDanger && Random.value < rescueRate) return Random.Range(0, 2);
            
            if(_totalWeight == -1) InitializeWeights();
            if (_totalWeight <= 0) return 0;

            int result = RollWheel();
            if (result == lastLevel && _spawnableIndices.Count > 1)
            {
                int secondTry = RollWheel();
                if (secondTry != lastLevel) result = secondTry;
            }

            return result;
        }

        public FruitInfo GetInfo(int level) => (level >= 0 && level < fruits.Length) ? fruits[level] : null;
        public int MaxLevel() => fruits.Length - 1;

        private void InitializeWeights()
        {
            _spawnableIndices.Clear();
            _totalWeight = 0;
            for (int i = 0; i < fruits.Length; i++)
            {
                if (fruits[i].isSpawnable && fruits[i].spawnWeight > 0)
                {
                    _spawnableIndices.Add(i);
                    _totalWeight += fruits[i].spawnWeight;
                }
            }
        }

        private int RollWheel()
        {
            if (_spawnableIndices.Count == 0) return 0;
            
            int randomValue = Random.Range(0, _totalWeight);
            int currentSum = 0;
            foreach (var index in _spawnableIndices)
            {
                currentSum += fruits[index].spawnWeight;
                if(randomValue < currentSum) return index;
            }
            return _spawnableIndices[0];
        }
    }

}
 