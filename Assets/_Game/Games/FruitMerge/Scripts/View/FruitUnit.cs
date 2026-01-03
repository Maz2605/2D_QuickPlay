using System;
using _Game.Games.FruitMerge.Scripts.Config;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.View
{
    [RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D))]
    public class FruitUnit : MonoBehaviour
    {
        public int Level { get; private set; }
        public bool IsMerge { get; private set; }

        private SpriteRenderer _renderer;
        private CircleCollider2D _collider;
        private Rigidbody2D _rb;

        public event Action<FruitUnit, Collision2D> OnCollisionWithFruit;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<CircleCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Initialize(int level, FruitMergeGameConfigSO.FruitInfo info)
        {
            Level = level;
            IsMerge = false;

            _renderer.sprite = info.visual;
            transform.localScale = Vector3.one * info.scale;

            _collider.radius = info.physicRadius;
            _rb.mass = info.mass;
            
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _collider.enabled = true;
            
            gameObject.SetActive(true);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log($"Va chạm với: {other.gameObject.name} - Tag: {other.gameObject.tag}");
            if(IsMerge) return;

            if (other.gameObject.CompareTag("Fruits"))
            {
                OnCollisionWithFruit?.Invoke(this, other);
            }
        }

        public void MarkAsMerge()
        {
            IsMerge = true;
            _collider.enabled = false;
        }
    }
}