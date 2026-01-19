using UnityEngine;
using System;
using System.Linq;
using _Game.Core.Scripts.GameSystem; 
using _Game.Core.Scripts.SkillSystem;
using _Game.Core.Scripts.Input;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.View;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public partial class FruitGameController
    {
        [Header("--- SKILL SETTINGS ---")]
        [SerializeField] private LayerMask fruitLayer; 

        public event Action<BaseSkillSO> OnSkillExecutionFinished;

        private FruitSkillSO _pendingSkillConfig; 

        public bool CanUseSkill(BaseSkillSO skillConfig)
        {
            if (CurrentGlobalState != GlobalGameState.Playing) return false;
            if (_localState != FruitLocalState.Idle) return false;

            return skillConfig is FruitSkillSO;
        }

        public bool TryExecuteSkill(BaseSkillSO skillConfig)
        {
            if (skillConfig is FruitSkillSO fruitSkill)
            {
                switch (fruitSkill.type)
                {
                    case FruitSkillType.HammerTarget:
                        EnterSkillMode(fruitSkill);
                        return true;

                    case FruitSkillType.ClearSmallest:
                        ExecuteClearSmallest(fruitSkill);
                        OnSkillExecutionFinished?.Invoke(skillConfig);
                        return true;
                }
            }
            return false;
        }
        
        private void EnterSkillMode(FruitSkillSO config)
        {
            _localState = FruitLocalState.UsingSkill;
            _pendingSkillConfig = config;
            
            spawner.SetInputActive(false); 
            
            Debug.Log($"[Skill Mode] Activated: {config.skillName}. Tap a fruit!");
        }

        private void ExitSkillMode()
        {
            _localState = FruitLocalState.Idle;
            _pendingSkillConfig = null;
            spawner.SetInputActive(true);
        }

        private void HandleTouchInput(Vector2 screenPos)
        {
            if (_localState != FruitLocalState.UsingSkill) return;

            Vector3 worldPos = InputManager.Instance.GetWorldPosition();
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, fruitLayer);

            if (hit.collider != null)
            {
                FruitUnit target = hit.collider.GetComponent<FruitUnit>();
                
                if (target != null && _activeFruits.Contains(target))
                {
                    if (_pendingSkillConfig != null && _pendingSkillConfig.vfxPrefab != null)
                    {
                        Instantiate(_pendingSkillConfig.vfxPrefab, target.transform.position, Quaternion.identity);
                    }

                    DespawnFruit(target);

                    OnSkillExecutionFinished?.Invoke(_pendingSkillConfig);
                    ExitSkillMode();
                }
            }
        }

        private void ExecuteClearSmallest(FruitSkillSO config)
        {
            int count = config.clearCount;
            
            var targets = _activeFruits.OrderBy(f => f.Level).Take(count).ToList();

            if (targets.Count > 0)
            {
                foreach (var fruit in targets)
                {
                    if (config.vfxPrefab != null)
                        Instantiate(config.vfxPrefab, fruit.transform.position, Quaternion.identity);

                    DespawnFruit(fruit);
                }
                Debug.Log($"[Skill] Cleared {targets.Count} fruits.");
            }
        }
    }
}