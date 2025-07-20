using _Script.DesignPattern.ObjectPooling;
using _Script.DesignPattern.Singleton;
using UnityEngine;

namespace _Script.FruitMergeGame
{
    public class FruitManager : Singleton<FruitManager>
    {
        [SerializeField] private GameObject fruitPrefab;

        public void SpawnFruit(int level, Vector3 position)
        {
            GameObject obj = PoolingManager.Instance.Spawn(fruitPrefab, position, Quaternion.identity);
            Fruits fruit = obj.GetComponent<Fruits>();
            fruit.Init(level);
        }

        public void MergeFruits(Fruits fruitA, Fruits fruitB)
        {
            int newLevel = fruitA.fruitLevel + 1;
            if (newLevel > FruitSpriteManager.Instance.MaxFruitLevel) return;

            Vector2 newPosition = new Vector2(
                (fruitA.transform.position.x + fruitB.transform.position.x) / 2f,
                Mathf.Max(fruitA.transform.position.y, fruitB.transform.position.y) + 0.2f // hoặc +offset tuỳ game
            );
        
            PoolingManager.Instance.Despawn(fruitA.gameObject);
            PoolingManager.Instance.Despawn(fruitB.gameObject);

            SpawnFruit(newLevel, newPosition);
        }
    }
}
