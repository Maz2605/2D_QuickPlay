using UnityEngine;
using System.Collections.Generic;

namespace _Game.Games.WaterSort.Scripts.Config
{
    [System.Serializable]
    public class BottleSetupData
    {
        public List<int> colors; 
    }

    [CreateAssetMenu(fileName = "Level_New", menuName = "Games/WaterSort/Level Config")]
    public class LevelConfigSO : ScriptableObject
    {
        public string nameLevel = "Level_";
        [Header("1. Cấu hình Chung")]
        public int bottleCapacity = 4;
        
        [Header("2. Bố cục (Layout)")]
        [Tooltip("Số chai trên từng hàng. Vd: [3, 4] là 2 hàng, tổng 7 chai.")]
        public List<int> bottlesPerRow; 

        public float spacingX = 1.5f;
        public float spacingY = 2.5f;

        [Header("3. Cấu hình Sinh Màu (Generator)")]
        [Tooltip("Số lượng màu dùng (Phải nhỏ hơn tổng số chai)")]
        public int colorCount = 5; 
        
        [Tooltip("Độ khó: Số bước trộn")]
        public int shuffleSteps = 20;

        [Header("4. Dữ liệu Màu (Kết quả)")]
        public List<BottleSetupData> bottles;
    }
}