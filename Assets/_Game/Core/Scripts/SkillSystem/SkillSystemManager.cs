using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Scripts.SkillSystem
{
    public class SkillSystemManager : MonoBehaviour
    {
        [Header("Data Configuration")]
        [Tooltip("Kéo các file SO Skill vào đây theo thứ tự mong muốn")]
        [SerializeField] private List<BaseSkillSO> skillDataList; 

        [Header("UI References (Manual)")]
        [Tooltip("Kéo các Button UI trên màn hình vào đây (Thứ tự phải khớp với Data)")]
        [SerializeField] private List<UISkillSlot> uiSlotsOnScreen;

        private IGameSkillExecutor _gameExecutor;

        public void Initialize(IGameSkillExecutor executor)
        {
            _gameExecutor = executor;
            
            _gameExecutor.OnSkillExecutionFinished += HandleSkillFinished;

            SetupUI();
        }

        private void OnDestroy()
        {
            if (_gameExecutor != null)
                _gameExecutor.OnSkillExecutionFinished -= HandleSkillFinished;
        }

        private void SetupUI()
        {
            for (int i = 0; i < uiSlotsOnScreen.Count; i++)
            {
                if (i < skillDataList.Count)
                {
                    UISkillSlot slot = uiSlotsOnScreen[i];
                    BaseSkillSO data = skillDataList[i];

                    slot.gameObject.SetActive(true);
                    slot.Initialize(data, OnSkillRequested);
                }
                else
                {
                    uiSlotsOnScreen[i].gameObject.SetActive(false);
                }
            }
        }

        // Khi người chơi bấm nút trên UI
        private void OnSkillRequested(BaseSkillSO data)
        {
            if (_gameExecutor == null) return;
            if (!_gameExecutor.CanUseSkill(data)) return;

            bool accepted = _gameExecutor.TryExecuteSkill(data);
            
            if (accepted)
            {
                Debug.Log($"[Skill System] Game accepted: {data.skillName}");
            }
        }

        // Khi Controller báo đã dùng xong skill
        private void HandleSkillFinished(BaseSkillSO data)
        {
            int index = skillDataList.IndexOf(data);
            
            if (index >= 0 && index < uiSlotsOnScreen.Count)
            {
                uiSlotsOnScreen[index].StartCooldown();
                
                // TODO: Trừ tiền thật sự ở đây
                Debug.Log($"[Skill System] Deduct Money: {data.price}");
            }
        }
    }
}