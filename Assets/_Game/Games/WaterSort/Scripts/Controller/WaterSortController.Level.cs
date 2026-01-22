using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using _Game.Games.WaterSort.Scripts.View;
using _Game.Games.WaterSort.Scripts.Model;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using _Game.Games.WaterSort.Scripts.Config;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public partial class WaterSortController
    {
        public void LoadLevel(LevelConfigSO levelData)
        {
            _localState = WaterSortLocalState.Intro; 
            _undoStack.Clear(); 
            ClearCurrentLevel();

            if (gameHUD)
            {
                string levelName = string.IsNullOrEmpty(levelData.nameLevel) ? "Level" : levelData.nameLevel;
                gameHUD.UpdateLevelName(levelName);
            }

            SpawnBottles(levelData);
            _localState = WaterSortLocalState.Idle;
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
             bot.Initialize(model); 
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
            _currentSelectedBottle=null; 
            _isProcessingHint=false; 
            _isBusy=false; 
        }

        // --- ALGORITHMS (Hint & Reshuffle) ---
        public void ShowHint()
        {
            if (_localState != WaterSortLocalState.Idle || _isBusy || _isProcessingHint) return;
            _isProcessingHint = true;
            
            // Logic tìm hint... (Giữ nguyên code cũ)
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
            _localState = WaterSortLocalState.Reshuffling;
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
                _undoStack.Clear(); 
                DOVirtual.DelayedCall(0.6f, () => { 
                    _localState = WaterSortLocalState.Idle; 
                    _isBusy = false; 
                });
            } else {
                if(LevelManager.Instance) LevelManager.Instance.OnClickResetGame();
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
                tempBottles[UnityEngine.Random.Range(0, availableIndices.Count)].Add(color);
            }
            for(int i=0; i<_activeBottles.Count; i++) _activeBottles[i].Model.ForceSetLiquids(tempBottles[i]);
            return true;
        }

        private void ShuffleList<T>(List<T> list) { 
            int n = list.Count; 
            while (n > 1) { 
                n--; int k = UnityEngine.Random.Range(0, n + 1); 
                (list[k], list[n]) = (list[n], list[k]); 
            } 
        }
    }
}