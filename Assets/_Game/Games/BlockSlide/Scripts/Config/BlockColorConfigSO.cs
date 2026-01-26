using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Games.BlockSlide.Scripts.Config
{
    [CreateAssetMenu(fileName = "BlockColorConfigSO", menuName = "Games/BlockSlide/BlockColorConfig")]
    public class BlockColorConfigSO: ScriptableObject
    {
        [Header("Block Color")]
        public Color defaultBackgroundColor = Color.grey;
        public Color defaultTextColor = Color.white;

        [Header("Block Style")]
        public List<BlockStyle> blockStyles = new List<BlockStyle>();
            
        public BlockStyle GetStyle(int value)
        {
            var style = blockStyles.Find(x => x.value == value);
            if (style.value == value) return style;
            
            if (blockStyles.Count > 0) return blockStyles[blockStyles.Count - 1];

            return new BlockStyle { value = value, backgroundColor = defaultBackgroundColor, textColor = defaultTextColor };
        }
        
    }
    [Serializable]
    public struct BlockStyle
    {
        public int value;           
        public Color backgroundColor;
        public Color textColor;
    }
}