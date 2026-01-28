using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.View;
using DG.Tweening;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public partial class FruitGameController
    {
        protected override void OnResetGameplay()
        {
            ResetGameLogic();
        }

        protected override void OnResumeGameplay()
        {
            base.OnResumeGameplay();
            ResumeGame();
        }
        
        private void OnFruitCollisionReceived(FruitCollisionPayload payload)
        {
            var fruitA = payload.FruitA;
            var fruitB = payload.FruitB;
            
            if (_currentState != FruitMergeState.Playing) return;
            if (fruitA == null || fruitB == null) return;
            
            if (fruitA.Level != fruitB.Level) return;
            if (fruitA.Level >= config.MaxLevel()) return;
            
            ProcessMerge(fruitA, fruitB);
        }

        // --- LOCAL STATE LOGIC ---

        private void PauseGame()
        {
            if (_currentState == FruitMergeState.Playing)
            {
                ChangeState(FruitMergeState.Paused);
                Time.timeScale = 0; 
                base.RequestPause();
            }
        }

        private void ResumeGame()
        {
            if (_currentState == FruitMergeState.Paused)
            {
                Time.timeScale = 1;
                ChangeState(FruitMergeState.Playing);
            }
        }

        private void StartGame()
        {
            ChangeState(FruitMergeState.Playing);
        }

        private void ResetGameLogic()
        {
            DOTween.KillAll();
            for (int i = _activeFruits.Count - 1; i >= 0; i--) 
            {
                DespawnFruit(_activeFruits[i]); 
            }
            _activeFruits.Clear();
            
            if (deadZone) deadZone.ResetZone();
            spawner.ResetSpawner();
            _scoreManager.ResetScore();
            
            ChangeState(FruitMergeState.Playing);
        }

        private void ChangeState(FruitMergeState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;
            EventManager<FruitMergeEventID>.Post(FruitMergeEventID.GameStateChanged, _currentState);
        }

        private void HandleGameOver()
        {
            if (_currentState == FruitMergeState.GameOver) return;
            
            ChangeState(FruitMergeState.GameOver);
            spawner.SetInputActive(false);

            _scoreManager.UpdateLeaderBoard();

            var payload = new GameOverPayLoad(
                _scoreManager.CurrentScore, 
                _scoreManager.GetTopScores()
            );
            EventManager<FruitMergeEventID>.Post(FruitMergeEventID.ShowGameOver, payload);
        }

        // --- MERGE LOGIC ---

        private void ProcessMerge(FruitUnit fruitA, FruitUnit fruitB)
        {
            fruitA.MarkAsMerging();
            fruitB.MarkAsMerging();
            
            Vector3 midPos = (fruitA.transform.position + fruitB.transform.position) / 2f;
            
            if (mergeEffectPrefab) 
                PoolingManager.Instance.Spawn(mergeEffectPrefab, midPos, Quaternion.identity);

            Sequence sequence = DOTween.Sequence();
            sequence.Join(fruitA.transform.DOMove(midPos, 0.1f));
            sequence.Join(fruitB.transform.DOMove(midPos, 0.1f));
            sequence.Join(fruitA.transform.DOScale(0f, 0.1f));
            sequence.Join(fruitB.transform.DOScale(0f, 0.1f));

            sequence.OnComplete(() =>
            {
                DespawnFruit(fruitA); 
                DespawnFruit(fruitB);

                if (_currentState != FruitMergeState.Playing) return;

                int nextLevel = fruitA.Level + 1; 
                int baseScore = config.GetInfo(nextLevel).scoreValue;
                
                _scoreManager.HandleMergeScore(baseScore);
                
                EventManager<FruitMergeEventID>.Post(FruitMergeEventID.FruitMerged, nextLevel);
                
                SpawnFruitsInternal(nextLevel, midPos, true);
            });
        }
        
        private void DespawnFruit(FruitUnit fruit)
        {
            if (fruit == null) return;
            
            if (_activeFruits.Contains(fruit)) _activeFruits.Remove(fruit);
            
            fruit.transform.DOKill();
            fruit.ResetPhysics(); 
            PoolingManager.Instance.Despawn(fruit.gameObject);
        }

        private void HandleDangerUpdate(float percent)
        {
             EventManager<FruitMergeEventID>.Post(FruitMergeEventID.DangerWarning, percent > 0.8f);
        }
    }
}