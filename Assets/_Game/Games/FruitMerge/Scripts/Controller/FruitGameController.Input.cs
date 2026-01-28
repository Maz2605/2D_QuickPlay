using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Core.Scripts.Utils.DesignPattern.ObjectPooling;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.Model;
using _Game.Games.FruitMerge.Scripts.View;
using DG.Tweening;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public partial class FruitGameController
    {
        private void HandleTouchStart(Vector2 pos) { if (_currentState == FruitMergeState.Playing) spawner.HandleStart(pos); }
        private void HandleTouchMove(Vector2 pos)  { if (_currentState == FruitMergeState.Playing) spawner.HandleMove(pos); }
        private void HandleTouchEnd(Vector2 pos)   { if (_currentState == FruitMergeState.Playing) spawner.HandleRelease(pos); }
        
        private void OnSpawnRequest(int level, Vector3 position)
        {
            if (_currentState != FruitMergeState.Playing) return;
            SpawnFruitsInternal(level, position, false);
            EventManager<FruitMergeEventID>.Post(FruitMergeEventID.FruitDropped);
        }

        public FruitUnit SpawnFruitsInternal(int level, Vector3 position, bool isMergeResult)
        {
            GameObject fruitObj = PoolingManager.Instance.Spawn(fruitPrefab, position, Quaternion.identity, fruitContainer);
            FruitUnit fruitUnit = fruitObj.GetComponent<FruitUnit>();
            
            var fruitData = new FruitModel(level, fruitObj.GetInstanceID());
            var info = config.GetInfo(level);
            fruitUnit.Initialize(fruitData, info);
            _activeFruits.Add(fruitUnit); 
            
            if (isMergeResult)
            {
                fruitUnit.transform.localScale = Vector3.one;
                fruitUnit.transform.DOScale(Vector3.one * info.scale, 0.4f).SetEase(Ease.OutBounce);
            }
            else
            {
                fruitUnit.transform.localScale = Vector3.one * info.scale;
            }
    
            return fruitUnit;
        }
    }
}