using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using _Game.Core.Scripts.GameSystem;
using _Game.Core.Scripts.Input;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using _Game.Core.Scripts.SkillSystem;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.View;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public enum FruitLocalState { Intro, Idle, Dropping, UsingSkill, Paused, GameOver, Resetting }
    
    public partial class FruitGameController : BaseGameController, IGameSkillExecutor
    {
        [Header("--- CORE CONFIG & REFS ---")] 
        [SerializeField] private FruitMergeGameConfigSO config;
        [SerializeField] private FruitSpawner spawner;
        [SerializeField] private GameObject fruitPrefab;
        [SerializeField] private Transform fruitContainer;
        [SerializeField] private GameObject mergeEffectPrefab;

        [Header("--- GAME OVER LOGIC ---")]
        [SerializeField] private FruitDeadZone deadZone; 

        [Header("--- UI & AUDIO ---")]
        [SerializeField] private FruitGameHUD gameHUD;
        [SerializeField] private FruitAudioController fruitAudioController;
        
        public event Action<int> OnFruitMerged;
        
        private FruitScoreManager _scoreManager;
        private FruitLocalState _localState = FruitLocalState.Intro;
        
        private List<FruitUnit> _activeFruits = new List<FruitUnit>(); 
        
        public override GlobalGameState CurrentGlobalState
        {
            get
            {
                switch (_localState)
                {
                    case FruitLocalState.Intro:
                    case FruitLocalState.Idle:
                    case FruitLocalState.Dropping:
                    case FruitLocalState.UsingSkill:
                        return GlobalGameState.Playing;
                    case FruitLocalState.Paused: return GlobalGameState.Paused;
                    case FruitLocalState.GameOver: return GlobalGameState.GameOver;
                    case FruitLocalState.Resetting: return GlobalGameState.Resetting;
                    default: return GlobalGameState.Loading;
                }
            }
        }
        
        private void Start()
        {
            // Init Data
            _scoreManager = new FruitScoreManager(config.gameID);
            _scoreManager.OnScoreChanged += HandleScoreChanged;
            _scoreManager.OnHighScoreChanged += HandleHighScoreChanged;
            
            // Init UI
            if (gameHUD)
            {
                gameHUD.Initialize();
                gameHUD.UpdateScore(_scoreManager.CurrentScore);
                gameHUD.UpdateHighScore(_scoreManager.HighScore); 
                gameHUD.OnPauseClicked += RequestPause;
                gameHUD.OnReplayClicked += RequestReplay;
                gameHUD.OnHomeClicked += RequestBackHome;
            }
            
            // Init Logic
            spawner.Initialize(config, OnSpawnRequest);
            if (fruitAudioController) fruitAudioController.Initialize(this);
            InputManager.Instance.OnTouchStart += HandleTouchInput;
            
            var skillManager = FindObjectOfType<SkillSystemManager>();
            if (skillManager != null) skillManager.Initialize(this);
            
            // Subscribe DeadZone Event (Thay cho Coroutine Loop cũ)
            if (deadZone)
            {
                deadZone.OnGameOverTriggered += HandleGameOver;
                deadZone.OnDangerTimerUpdate += HandleDangerUpdate;
            }
            
            _localState = FruitLocalState.Idle;
        }

        private void OnDestroy()
        {
            _scoreManager?.Save();
            
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreChanged -= HandleScoreChanged;
                _scoreManager.OnHighScoreChanged -= HandleHighScoreChanged;
            }
            
            if (InputManager.Instance) InputManager.Instance.OnTouchStart -= HandleTouchInput;
            
            if (gameHUD)
            {
                gameHUD.OnPauseClicked -= RequestPause;
                gameHUD.OnReplayClicked -= RequestReplay;
                gameHUD.OnHomeClicked -= RequestBackHome;
            }

            if (deadZone)
            {
                deadZone.OnGameOverTriggered -= HandleGameOver;
                deadZone.OnDangerTimerUpdate -= HandleDangerUpdate;
            }
        }
        
        // --- GAMEPLAY LOGIC ---

        private void OnSpawnRequest(int level, Vector3 position)
        {
            if (_localState != FruitLocalState.Idle) return;
            SpawnFruitsInternal(level, position, false);
        }

        public FruitUnit SpawnFruitsInternal(int level, Vector3 position, bool isMergeResult)
        {
            GameObject fruitObj = PoolingManager.Instance.Spawn(fruitPrefab, position, Quaternion.identity, fruitContainer);
            FruitUnit fruit = fruitObj.GetComponent<FruitUnit>();

            var info = config.GetInfo(level);
            fruit.Initialize(level, info);

            _activeFruits.Add(fruit); 

            // Animation logic
            if (isMergeResult)
            {
                fruit.transform.localScale = Vector3.one;
                fruit.transform.DOScale(Vector3.one * info.scale, 0.4f)
                     .SetEase(Ease.OutBounce)
                     .SetLink(fruit.gameObject);
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
            if (_localState != FruitLocalState.Idle && _localState != FruitLocalState.Dropping) return;

            if (!collision2D.gameObject.TryGetComponent(out FruitUnit fruitB)) return;
            
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
            if (mergeEffectPrefab) PoolingManager.Instance.Spawn(mergeEffectPrefab, midPos, Quaternion.identity);

            Sequence sequence = DOTween.Sequence();
            sequence.Join(fruitA.transform.DOMove(midPos, 0.1f));
            sequence.Join(fruitB.transform.DOMove(midPos, 0.1f));
            sequence.Join(fruitA.transform.DOScale(0f, 0.1f));
            sequence.Join(fruitB.transform.DOScale(0f, 0.1f));

            sequence.OnComplete(() =>
            {
                if (_localState != FruitLocalState.Idle && _localState != FruitLocalState.Dropping) 
                {
                    DespawnFruit(fruitA); 
                    DespawnFruit(fruitB); 
                    return;
                }

                if (fruitA == null || fruitB == null) return;

                int nextLevel = fruitA.Level + 1;
                
                _scoreManager.AddScore(config.GetInfo(nextLevel).scoreValue);
                OnFruitMerged?.Invoke(nextLevel);
                
                DespawnFruit(fruitA); 
                DespawnFruit(fruitB);
                
                SpawnFruitsInternal(nextLevel, midPos, true);
            });
        }

        private void DespawnFruit(FruitUnit fruit)
        {
            if (fruit == null) return;
            fruit.OnCollisionWithFruit -= HandleCollision;
            
            if (_activeFruits.Contains(fruit)) _activeFruits.Remove(fruit);
            
            fruit.transform.DOKill(); 
            
            fruit.transform.localScale = Vector3.one;
            if (fruit.TryGetComponent(out Collider2D col)) col.enabled = true; 
            
            PoolingManager.Instance.Despawn(fruit.gameObject);
        }


        private void HandleGameOver()
        {
            if (_localState == FruitLocalState.GameOver) return;
            
            Debug.Log("GAME OVER TRIGGERED BY DEADZONE");
            _localState = FruitLocalState.GameOver;

            spawner.SetInputActive(false);
            
            _scoreManager.UpdateLeaderboard();
            var topScores = _scoreManager.GetTopScores();
            
            if (gameHUD)
            {
                gameHUD.SetUIState(GlobalGameState.GameOver, _scoreManager.CurrentScore, topScores);
            }
        }
        
        private void HandleDangerUpdate(float dangerPercent)
        {
            // if (gameHUD) gameHUD.SetDangerEffect(dangerPercent);
        }

        protected override void OnResetGameplay()
        {
            _localState = FruitLocalState.Resetting;
            
            if (deadZone) deadZone.ResetZone();

            ExitSkillMode(); 
            DOTween.KillAll(); 
            
            // Clear map: Loop ngược để remove an toàn
            for (int i = _activeFruits.Count - 1; i >= 0; i--) 
            {
                DespawnFruit(_activeFruits[i]); 
            }
            _activeFruits.Clear();
            
            _scoreManager.ResetScore();
            spawner.ResetSpawner(); 

            if (gameHUD) 
            {
                gameHUD.SetUIState(GlobalGameState.Playing); 
                gameHUD.UpdateScore(0); 
                gameHUD.ShowCombo(0); 
            }

            _localState = FruitLocalState.Idle;
        }
        
        protected override void OnResumeGameplay()
        {
            ResumeGame(); 
        }
        
        protected new void RequestPause()
        {
            if (_localState == FruitLocalState.Paused) return;
            _localState = FruitLocalState.Paused;
            gameHUD.SetUIState(GlobalGameState.Paused);
            base.RequestPause(); 
        }
        
        public void ResumeGame()
        {
            if (_localState == FruitLocalState.GameOver || _localState == FruitLocalState.Resetting) return;
            _localState = FruitLocalState.Idle; 
            if (gameHUD) 
            {
                gameHUD.SetUIState(GlobalGameState.Playing);
            }
        }
        
        private void HandleScoreChanged(int score) { if (gameHUD) gameHUD.UpdateScore(score); }
        private void HandleHighScoreChanged(int highScore) { if (gameHUD) gameHUD.UpdateHighScore(highScore); }
    }
}