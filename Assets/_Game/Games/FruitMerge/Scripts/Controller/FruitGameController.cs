using System;
using System.Collections.Generic; // Cần dùng List
using _Game.Core.Scripts.GameSystem; 
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.View;
using DG.Tweening;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    // Thêm trạng thái game để quản lý logic chặt chẽ hơn
    public enum GameState { Playing, Paused, GameOver, Resetting }

    public class FruitGameController : BaseGameController
    {
        [Header("Dependencies")] 
        [SerializeField] private FruitMergeGameConfigSO config;
        [SerializeField] private FruitSpawner spawner; // Giả sử class này xử lý input thả trái cây
        [SerializeField] private GameObject fruitPrefab;
        [SerializeField] private Transform fruitContainer;
        [SerializeField] private GameObject mergeEffectPrefab;

        [Header("UI View")]
        [SerializeField] private FruitGameHUD gameHUD;

        [Header("Audio")] 
        [SerializeField] private FruitAudioController fruitAudioController;

        public event Action<int> OnFruitMerged;

        private FruitScoreManager _scoreManager;
        private GameState _currentState = GameState.Playing;
        
        private List<FruitUnit> _activeFruits = new List<FruitUnit>();

        private void Start()
        {
            _scoreManager = new FruitScoreManager(config.gameID);
            _scoreManager.OnScoreChanged += HandleScoreChanged;
            _scoreManager.OnHighScoreChanged += HandleHighScoreChanged;
            
            if (gameHUD != null)
            {
                gameHUD.UpdateScore(_scoreManager.CurrentScore);
                gameHUD.UpdateHighScore(_scoreManager.HighScore); 
                gameHUD.OnPauseClicked += RequestPause;
                gameHUD.OnReplayClicked += RequestReplay;
                gameHUD.OnHomeClicked += RequestBackHome;
            }
            
            spawner.Initialize(config, OnSpawnRequest);
            if (fruitAudioController != null) fruitAudioController.Initialize(this);
            
            _currentState = GameState.Playing;
        }

        private void OnDestroy()
        {
            _scoreManager?.Save();
            if (gameHUD != null)
            {
                gameHUD.OnPauseClicked -= RequestPause;
                gameHUD.OnReplayClicked -= RequestReplay;
                gameHUD.OnHomeClicked -= RequestBackHome;
            }
        }

        protected override void OnResetGameplay()
        {
            _currentState = GameState.Resetting;

            DOTween.KillAll(); 
            
            for (int i = _activeFruits.Count - 1; i >= 0; i--)
            {
                DespawnFruit(_activeFruits[i]); 
            }
            
            _activeFruits.Clear();
            _scoreManager.ResetScore();
            spawner.ResetSpawner(); 

            if (gameHUD != null)
            {
                gameHUD.UpdateScore(0);
                gameHUD.ShowCombo(0);
            }

            _currentState = GameState.Playing;
        }

        // --- Gameplay Logic ---

        private void OnSpawnRequest(int level, Vector3 position)
        {
            if (_currentState != GameState.Playing) return;
            SpawnFruitsInternal(level, position, false);
        }

        public FruitUnit SpawnFruitsInternal(int level, Vector3 position, bool isMergeResult)
        {
            GameObject fruitSpawn = PoolingManager.Instance.Spawn(fruitPrefab, position, Quaternion.identity, fruitContainer);
            FruitUnit fruit = fruitSpawn.GetComponent<FruitUnit>();

            var info = config.GetInfo(level);
            fruit.Initialize(level, info);

            _activeFruits.Add(fruit);

            if (isMergeResult)
            {
                fruit.transform.localScale = Vector3.one;
                fruit.transform.DOScale(Vector3.one * info.scale, 0.4f).SetEase(Ease.OutBounce).SetLink(fruit.gameObject);
            }
            else
            {
                fruit.transform.localScale = Vector3.one * info.scale;
            }

            fruit.OnCollisionWithFruit += HandleCollision;
            
            return fruit;
        }

        private void HandleCollision(FruitUnit fruitA, Collision2D collision2D)
        {
            if (_currentState != GameState.Playing) return;

            FruitUnit fruitB = collision2D.gameObject.GetComponent<FruitUnit>();
            if (fruitB == null) return;

            if (fruitA.Level != fruitB.Level) return;
            if (fruitA.Level >= config.MaxLevel()) return;
            if (fruitA.GetInstanceID() > fruitB.GetInstanceID()) return; 

            ProcessMerge(fruitA, fruitB);
        }

        private void ProcessMerge(FruitUnit fruitA, FruitUnit fruitB)
        {
            fruitA.MarkAsMerge();
            fruitB.MarkAsMerge();
            
            if (fruitA.TryGetComponent(out Collider2D colA)) colA.enabled = false;
            if (fruitB.TryGetComponent(out Collider2D colB)) colB.enabled = false;

            Vector3 midPos = (fruitA.transform.position + fruitB.transform.position) / 2f;

            if (mergeEffectPrefab != null)
            {
                PoolingManager.Instance.Spawn(mergeEffectPrefab, midPos, Quaternion.identity);
            }

            Sequence sequence = DOTween.Sequence();
            sequence.Join(fruitA.transform.DOMove(midPos, 0.1f));
            sequence.Join(fruitB.transform.DOMove(midPos, 0.1f));
            sequence.Join(fruitA.transform.DOScale(0f, 0.1f));
            sequence.Join(fruitB.transform.DOScale(0f, 0.1f));

            sequence.OnComplete(() =>
            {
                if (_currentState != GameState.Playing) 
                {
                    DespawnFruit(fruitA);
                    DespawnFruit(fruitB);
                    return;
                }

                int nextLevel = fruitA.Level + 1;
                
                _scoreManager.AddScore(config.GetInfo(nextLevel).scoreValue);
                OnFruitMerged?.Invoke(nextLevel);

                DespawnFruit(fruitA);
                DespawnFruit(fruitB);

                SpawnFruitsInternal(nextLevel, midPos, true);
            });
        }

        public void DespawnFruit(FruitUnit fruit)
        {
            if (fruit == null) return;
            
            fruit.OnCollisionWithFruit -= HandleCollision;
            
            if (_activeFruits.Contains(fruit))
            {
                _activeFruits.Remove(fruit);
            }

            fruit.transform.DOKill(); 
            fruit.transform.localScale = Vector3.one;
            if (fruit.TryGetComponent(out Collider2D col)) col.enabled = true; 

            PoolingManager.Instance.Despawn(fruit.gameObject);
        }

        private void HandleScoreChanged(int score) { if (gameHUD) gameHUD.UpdateScore(score); }
        private void HandleHighScoreChanged(int highScore) { if (gameHUD) gameHUD.UpdateHighScore(highScore); }
    }
}