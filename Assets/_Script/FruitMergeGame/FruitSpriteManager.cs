using _Script.DesignPattern.Singleton;
using UnityEngine;

namespace _Script.FruitMergeGame
{
    public class FruitSpriteManager : Singleton<FruitSpriteManager>
    {
        [SerializeField]
        private Sprite[] fruitSprites;
    
        public Sprite GetFruitSprite(int level)
        {
            if (level < 0 || level >= fruitSprites.Length)
            {
                Debug.LogWarning("Fruit level is out of bounds.");
                return null;
            }
            return fruitSprites[level];
        }
    
        public int MaxFruitLevel => fruitSprites.Length - 1;
    }
}
