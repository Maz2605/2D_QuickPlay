    using System;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.View;
using _Script.DesignPattern.ObjectPooling;
using DG.Tweening;
using Unity.Mathematics;
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
        [SerializeField] private GameObject mergeEffectPrefab;
        
        [Header("Audio")]
        [SerializeField] private FruitAudioController fruitAudioController;
        
        public event Action<int> OnFruitMerged;

        private FruitScoreManager _scoreManager;

        private void Start()
        {
            _scoreManager = new FruitScoreManager(config.gameID);
            _scoreManager.OnScoreChanged += (s) => Debug.Log($"Current Score: {s}");
            _scoreManager.OnHighScoreChanged += (s) => Debug.Log($"Highest core: {s}");
            spawner.Initialize(config,OnSpawnRequest );
            
            if(fruitAudioController != null) fruitAudioController.Initialize(this);
        }

        private void OnDestroy() => _scoreManager?.Save();
        
        private void OnSpawnRequest(int level, Vector3 position) => SpawnFruitsInternal(level, position, false);

        private void SpawnFruitsInternal(int level, Vector3 position, bool isMergeResult)
        {
            GameObject fruitSpawn =
                PoolingManager.Instance.Spawn(fruitPrefab, position, Quaternion.identity, fruitContainer);
            FruitUnit fruit = fruitSpawn.GetComponent<FruitUnit>();

            var info = config.GetInfo(level);
            fruit.Initialize(level, config.GetInfo(level));

            if (isMergeResult)
            {
                fruit.transform.localScale = Vector3.one;
                fruit.transform.DOScale(Vector3.one * info.scale, 0.4f ).SetEase(Ease.OutBounce);
            }
            else
            {
                fruit.transform.localScale = Vector3.one * info.scale;
            }
            
            fruit.OnCollisionWithFruit += HandleCollision;
        }

        private void HandleCollision(FruitUnit fruitA, Collision2D collision2D)
        {
            FruitUnit fruitB = collision2D.gameObject.GetComponent<FruitUnit>();
            if(fruitB == null) return;
            
            if(fruitA.Level != fruitB.Level) return;
            if(fruitA.Level >= config.MaxLevel()) return;
            if(fruitA.GetInstanceID() > fruitB.GetInstanceID()) return;
            
            fruitA.MarkAsMerge();
            fruitB.MarkAsMerge();
            Vector3 midPos = (fruitA.transform.position + fruitB.transform.position) / 2f;

            if (mergeEffectPrefab != null)
            {
                PoolingManager.Instance.Spawn(mergeEffectPrefab, midPos, Quaternion.identity);
            }
            
            Sequence sequence = DOTween.Sequence();

            if (fruitA.GetComponent<Collider2D>()) fruitA.GetComponent<Collider2D>().enabled = false;
            if (fruitB.GetComponent<Collider2D>()) fruitB.GetComponent<Collider2D>().enabled = false;
            
            sequence.Join(fruitA.transform.DOMove(midPos, 0.1f));
            sequence.Join(fruitB.transform.DOMove(midPos, 0.1f));
            sequence.Join(fruitA.transform.DOScale(0f, 0.1f));
            sequence.Join(fruitB.transform.DOScale(0f, 0.1f));

            sequence.OnComplete(() =>
            {
                Despawn(fruitA);
                Despawn(fruitB);

                int nextLevel = fruitA.Level + 1;
                _scoreManager.AddScore(config.GetInfo(nextLevel).scoreValue);
                
                OnFruitMerged?.Invoke(nextLevel);

                Despawn(fruitA);
                Despawn(fruitB);
                SpawnFruitsInternal(nextLevel, midPos, true);
            });

        }

        private void Despawn(FruitUnit fruit)
        {
            fruit.OnCollisionWithFruit -= HandleCollision;
            
            fruit.transform.localScale = Vector3.one;
            PoolingManager.Instance.Despawn(fruit.gameObject);
        }
    }
}