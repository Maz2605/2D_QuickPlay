using System;

namespace _Game.Core.Scripts.SkillSystem
{
    public interface IGameSkillExecutor
    {
        bool CanUseSkill(BaseSkillSO skillConfig);
        bool TryExecuteSkill(BaseSkillSO skillConfig);

        event Action<BaseSkillSO> OnSkillExecutionFinished;
    }
}