using _Game.Games.BlockSlide.Scripts.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Games.BlockSlide.Scripts.View
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI numberText;

        public void SetData(int value, BlockColorConfigSO config)
        {
            if (value == 0)
            {
                numberText.text = "";
                backgroundImage.color = config.defaultBackgroundColor;
            }
            else
            {
                BlockStyle style = config.GetStyle(value);
                
                backgroundImage.color = style.backgroundColor;
                numberText.text = value.ToString();
                numberText.color = style.textColor;
            }
        }
    }
}