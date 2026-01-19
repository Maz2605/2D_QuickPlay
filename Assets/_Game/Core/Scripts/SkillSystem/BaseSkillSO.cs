using UnityEngine;

namespace _Game.Core.Scripts.SkillSystem
{
    public abstract class BaseSkillSO : ScriptableObject
    {
        [Header("Core UI")]
        public string skillName;
        public Sprite icon;
        public string description;

        [Header("Core Rules")]
        public int price;          
        public float cooldownTime; 
        public bool isAdsReward;   
        
       public string SkillID => this.name;
    }
}