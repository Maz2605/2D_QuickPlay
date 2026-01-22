using UnityEngine;

namespace _Game.Games.WaterSort.Scripts.Config
{
    [CreateAssetMenu(fileName = "ColorConfig", menuName = "Games/WaterSort/ColorConfig")]
    public class ColorConfigSO : ScriptableObject
    {
        [Tooltip("Index của list này chính là ID của màu trong Logic")]
        public Color[] colors;

        public Color GetColor(int id)
        {
            if (id >= 0 && id < colors.Length)
                return colors[id];
            return Color.white;
        }
    }
}