using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using _Game.Games.WaterSort.Scripts.View;
using _Game.Games.WaterSort.Scripts.Model;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using _Game.Games.WaterSort.Scripts.Config;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public partial class WaterSortController
    {
        // --- BASE CONTROLLER OVERRIDES ---
        protected override void OnResetGameplay()
        {
             if (levelManager != null) 
             {
                 levelManager.OnClickResetGame();
             }
        }

        protected override void OnResumeGameplay()
        {
            base.OnResumeGameplay();
            Time.timeScale = 1f;
            if (_currentState == WaterSortState.Paused)
            {
                ChangeState(WaterSortState.Idle);
            }
        }

        // --- STATE MANAGEMENT ---
        private void ChangeState(WaterSortState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;
            EventManager<WaterSortEventID>.Post(WaterSortEventID.GameStateChanged, _currentState);
        }

        // --- LEVEL LOADING ---
        public void LoadLevel(LevelConfigSO levelData, int levelIndex)
        {
            Time.timeScale = 1f; 
            ChangeState(WaterSortState.Intro);
            
            _commandInvoker?.ClearHistory(); 
            ClearCurrentLevel();
            SpawnBottles(levelData);

            string levelName = levelData.nameLevel;
            if (string.IsNullOrEmpty(levelName))
            {
                levelName = $"Level {levelIndex + 1}"; 
            }

            EventManager<WaterSortEventID>.Post(WaterSortEventID.LevelLoaded, levelName);

            ChangeState(WaterSortState.Idle);
        }

        private void SpawnBottles(LevelConfigSO levelData)
        {
            int currentBottleIndex = 0;
            Vector3 currentRowPos = spawnPoint.position;
            
            for (int rowIndex = 0; rowIndex < levelData.bottlesPerRow.Count; rowIndex++)
            {
                int countInRow = levelData.bottlesPerRow[rowIndex];
                float startX = -(countInRow - 1) * levelData.spacingX / 2f;
                for (int i = 0; i < countInRow; i++) 
                {
                    if (currentBottleIndex >= levelData.bottles.Count) break;
                    Vector3 spawnPos = currentRowPos; 
                    spawnPos.x += startX + (i * levelData.spacingX);
                    
                    SpawnSingleBottle(spawnPos, currentBottleIndex, levelData);
                    currentBottleIndex++;
                }
                currentRowPos.y -= levelData.spacingY;
            }
        }

        private void SpawnSingleBottle(Vector3 pos, int index, LevelConfigSO levelData) 
        {
             BottleView bot = PoolingManager.Instance.Spawn(bottlePrefab, pos, Quaternion.identity, transform);
             BottleModel model = new BottleModel(levelData.bottleCapacity);
             foreach (int color in levelData.bottles[index].colors) model.Push(color);
             
             WaterSortGameConfig config = levelManager != null ? levelManager.GetSettings() : null;
             
             bot.Initialize(model, config); 
             
             bot.name = $"Bottle_{index}"; 
             _activeBottles.Add(bot);
        }

        private void ClearCurrentLevel() 
        { 
            foreach (var b in _activeBottles) 
            { 
                if(b)
                {
                    b.transform.DOKill(); 
                    b.SetSelected(false); 
                    PoolingManager.Instance.Despawn(b.gameObject);
                } 
            } 
            _activeBottles.Clear(); 
            _currentSelectedBottle = null; 
            _isProcessingHint = false; 
            _isBusy = false; 
        }

        // --- GAME LOGIC ---
        private void CheckGameState()
        {
            bool win = true;
            foreach(var b in _activeBottles) 
                if(!b.Model.IsEmpty && !b.Model.IsSolved()){ win = false; break; }
            
            if(win) 
            {
                HandleLevelWin();
                return;
            }

            if(IsDeadlock(true) && autoReshuffleOnDeadlock) 
            { 
                ChangeState(WaterSortState.Reshuffling);
                _isBusy = true; 
                DOVirtual.DelayedCall(2f, () => { if(this) ReshuffleBoard(); }); 
            }
            else 
            {
                ChangeState(WaterSortState.Idle);
            }
        }

        private void HandleLevelWin()
        {
            ChangeState(WaterSortState.Victory);
            EventManager<WaterSortEventID>.Post(WaterSortEventID.LevelWin);
        }

        private void RequestUndo()
        {
            if (_currentState != WaterSortState.Idle || !_commandInvoker.CanUndo() || _isBusy) return;
            
            _commandInvoker.Undo();
            foreach (var b in _activeBottles) b.UpdateVisuals();
            
            EventManager<WaterSortEventID>.Post(WaterSortEventID.UndoExecuted);
        }

        private void RequestHint()
        {
            if (_currentState == WaterSortState.Idle && !_isBusy) ShowHint();
        }
        
        private new void RequestPause() 
        {
            if (_currentState == WaterSortState.Idle || _currentState == WaterSortState.Pouring)
            {
                ChangeState(WaterSortState.Paused);
                Time.timeScale = 0;
                base.RequestPause(); 
            }
        }
        
        private void LoadNextLevel()
        {
             if (levelManager != null) levelManager.LoadNextLevel();
        }

        // --- ALGORITHMS ---
        public void ShowHint()
        {
            if (_currentState != WaterSortState.Idle || _isBusy || _isProcessingHint) return;
            _isProcessingHint = true;
            
            foreach (var s in _activeBottles) 
            {
                if (s.Model.IsEmpty || s.Model.IsSolved()) continue;
                foreach (var t in _activeBottles) 
                {
                    if (s == t || t.Model.IsSolved()) continue;
                    if (t.Model.CanPush(s.Model.TopColor)) 
                    {
                        if (t.Model.IsEmpty && s.Model.IsUniformColor()) continue;
                        if ((t.Model.Capacity - t.Model.Count) < s.Model.GetCountSameTopColor()) continue;
                        
                        s.transform.DOJump(s.transform.position, 0.5f, 1, 0.5f)
                            .OnComplete(() => _isProcessingHint = false);
                        return;
                    }
                }
            }
            ReshuffleBoard(); 
            _isProcessingHint = false;
        }

        private bool IsDeadlock(bool strict)
        {
            foreach (var s in _activeBottles) {
                if (s.Model.IsEmpty || s.Model.IsSolved()) continue;
                foreach (var t in _activeBottles) {
                    if (s == t || t.Model.IsSolved()) continue;
                    if (t.Model.CanPush(s.Model.TopColor)) {
                        if (strict) {
                            if (t.Model.IsEmpty && s.Model.IsUniformColor()) continue;
                            if ((t.Model.Capacity - t.Model.Count) < s.Model.GetCountSameTopColor()) continue;
                        }
                        return false; 
                    }
                }
            }
            return true; 
        }

        private void ReshuffleBoard()
        {
            ChangeState(WaterSortState.Reshuffling);
            List<int> allLiquids = new List<int>();
            foreach (var bot in _activeBottles) allLiquids.AddRange(bot.Model.ClearAndGetAllLiquids());

            bool success = false;
            for (int i = 0; i < 100; i++) {
                ShuffleList(allLiquids);
                if (TryDistributeRandomly(allLiquids)) {
                    if (!IsDeadlock(true)) { success = true; break; }
                }
                foreach (var bot in _activeBottles) bot.Model.ClearAndGetAllLiquids();
            }

            if (success) {
                foreach (var bot in _activeBottles) { 
                    bot.UpdateVisuals(); 
                    bot.transform.DOPunchRotation(new Vector3(0, 0, 15), 0.5f); 
                }
                _commandInvoker.ClearHistory(); 
                DOVirtual.DelayedCall(0.6f, () => { 
                    ChangeState(WaterSortState.Idle);
                    _isBusy = false; 
                });
            } else {
                if(levelManager != null) levelManager.OnClickResetGame();
            }
        }

        private bool TryDistributeRandomly(List<int> liquids) {
            Queue<int> pool = new Queue<int>(liquids);
            List<List<int>> tempBottles = new List<List<int>>();
            for(int i=0; i<_activeBottles.Count; i++) tempBottles.Add(new List<int>());
            while(pool.Count > 0) {
                int color = pool.Dequeue();
                List<int> availableIndices = new List<int>();
                for(int i=0; i<_activeBottles.Count; i++) if(tempBottles[i].Count < _activeBottles[i].Model.Capacity) availableIndices.Add(i);
                if(availableIndices.Count == 0) return false;
                tempBottles[Random.Range(0, availableIndices.Count)].Add(color);
            }
            for(int i=0; i<_activeBottles.Count; i++) _activeBottles[i].Model.ForceSetLiquids(tempBottles[i]);
            return true;
        }

        private void ShuffleList<T>(List<T> list) { 
            int n = list.Count; 
            while (n > 1) { 
                n--; int k = Random.Range(0, n + 1); 
                (list[k], list[n]) = (list[n], list[k]); 
            } 
        }
    }
}