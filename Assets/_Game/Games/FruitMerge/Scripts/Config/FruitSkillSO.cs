using UnityEngine;
using _Game.Core.Scripts.SkillSystem;

namespace _Game.Games.FruitMerge.Scripts.Config
{
    public enum FruitSkillType
    {
        HammerTarget,   
        ClearSmallest,  
        ShakeBoard      
    }

    [CreateAssetMenu(fileName = "NewFruitSkill", menuName = "Games/FruitMerge/Skill Config")]
    public class FruitSkillSO : BaseSkillSO
    {
        [Header("Fruit Specific Logic")]
        public FruitSkillType type;

        [Header("Optional Data")]
        public GameObject vfxPrefab; 
        public float effectRadius = 1.0f; 
        public int clearCount = 5;  
    }
}