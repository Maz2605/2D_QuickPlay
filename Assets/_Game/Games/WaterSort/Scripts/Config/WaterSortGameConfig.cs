using UnityEngine;

namespace _Game.Games.WaterSort.Scripts.Config
{
    [CreateAssetMenu(fileName = "WaterSortGameConfig", menuName = "Games/WaterSort/GameConfig")]
    public class WaterSortGameConfig : ScriptableObject
    {
        [Header("--- SYSTEM SETTINGS ---")]
        public string gameID = "water_sort_data";
        
        [Header("--- RESOURCES ---")]
        public ColorConfigSO colorConfig;
        
        [Header("--- ANIMATION SETTINGS ---")]
        public float moveSpeed = 18f;
        public float rotateSpeed = 200f;
        public float pourAngle = 50f;
        public Vector3 pourOffset = new Vector3(0.8f, 1f, 0f);

        [Header("--- WATER FILLING ---")]
        public float timePerLayer = 0.25f;

        [Header("--- GAMEPLAY LOGIC ---")]
        [Tooltip("Thời gian chờ trước khi tự động trộn lại (Giây)")]
        public float autoReshuffleDelay = 2.0f; 
    }
    
}