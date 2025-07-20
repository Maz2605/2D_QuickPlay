using UnityEngine;

namespace _Script.FruitMergeGame
{
    public class Fruits : MonoBehaviour
    {
        public int fruitLevel = 0;
        private SpriteRenderer spriteRenderer;
        private CircleCollider2D circleCollider;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            circleCollider = GetComponent<CircleCollider2D>();
        }
    
        public void Init(int level)
        {
            fruitLevel = level;
            UpdateSprite();
            UpdateCollider();
        }

        public void UpdateSprite()
        {
            Sprite sprite = FruitSpriteManager.Instance.GetFruitSprite(fruitLevel);
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }

        public void UpdateCollider()
        {
            if (spriteRenderer == null || circleCollider == null) return;
            float size = Mathf.Max(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
            circleCollider.radius = size / 2f * 0.9f;
            circleCollider.offset = Vector2.zero;
        }
    
    
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Fruits"))
            {
                var otherFruit = other.gameObject.GetComponent<Fruits>();
                if (otherFruit != null && otherFruit.fruitLevel == fruitLevel)
                {
                    if(this.GetInstanceID() < otherFruit.GetInstanceID())
                    {
                        FruitManager.Instance.MergeFruits(this, otherFruit);
                    }
                }
            }
        }
    }
}