using System;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.View;
using _Script.DesignPattern.ObjectPooling;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public class FruitGameEntryPoint : MonoBehaviour
    {
        [FormerlySerializedAs("gameConfig")]
        [Header("Dependencies")] 
        [SerializeField] private FruitMergeGameConfigSO config;
        [SerializeField] private FruitSpawner spawner;
        [SerializeField] private GameObject fruitPrefab;
        [SerializeField] private Transform fruitContainer;

        private FruitScoreManager _scoreManager;

        private void Start()
        {
            _scoreManager = new FruitScoreManager(config.gameID);
            _scoreManager.OnHighScoreChanged += (s) => Debug.Log($"Highscore: {s}");
            
            spawner.Initialize(config,OnSpawnRequest );
        }

        private void OnDestroy() => _scoreManager?.Save();
        
        private void OnSpawnRequest(int level, Vector3 position) => SpawnFruitsInternal(level, position);

        private void SpawnFruitsInternal(int level, Vector3 position)
        {
            GameObject fruitSpawn =
                PoolingManager.Instance.Spawn(fruitPrefab, position, Quaternion.identity, fruitContainer);
            FruitUnit fruit = fruitSpawn.GetComponent<FruitUnit>();
            
            fruit.Initialize(level, config.GetInfo(level));
            fruit.OnCollisionWithFruit += HandleCollision;
        }

        private void HandleCollision(FruitUnit fruitA, Collision2D collision2D)
        {
            Debug.Log("EntryPoint nhận tín hiệu Merge!");
            FruitUnit fruitB = collision2D.gameObject.GetComponent<FruitUnit>();
            if(fruitB == null) return;
            
            if(fruitA.Level != fruitB.Level) return;
            if(fruitA.Level >= config.MaxLevel()) return;
            if(fruitA.GetInstanceID() > fruitB.GetInstanceID()) return;
            
            fruitA.MarkAsMerge();
            fruitB.MarkAsMerge();
            Vector3 midPos = (fruitA.transform.position + fruitB.transform.position) / 2f;

            int nextLevel = fruitA.Level + 1;
            _scoreManager.AddScore(config.GetInfo(nextLevel).scoreValue);
            
            Despawn(fruitA);
            Despawn(fruitB);
            SpawnFruitsInternal(nextLevel, midPos);
        }

        private void Despawn(FruitUnit fruit)
        {
            fruit.OnCollisionWithFruit -= HandleCollision;
            PoolingManager.Instance.Despawn(fruit.gameObject);
        }
    }
}