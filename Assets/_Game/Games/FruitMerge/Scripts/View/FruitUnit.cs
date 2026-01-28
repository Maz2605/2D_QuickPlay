using System;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.Model;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.View
{
    [RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D))]
    public class FruitUnit : MonoBehaviour
    {
        public FruitModel Data { get; private set; }
        private SpriteRenderer _renderer;
        private CircleCollider2D _collider;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<CircleCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
        }
        
        public void Initialize(FruitModel data, FruitMergeGameConfigSO.FruitInfo info)
        {
     
            Data = data;
            ResetPhysics();

            _renderer.sprite = info.visual;
            transform.localScale = Vector3.one * info.scale;

            _collider.radius = info.physicRadius;
            _rb.mass = info.mass;
            
            gameObject.SetActive(true);
        }

        public void ResetPhysics()
        {
            if (Data != null) Data.isMerging = false;

            _collider.enabled = true;
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }

        public void MarkAsMerging()
        {
            if (Data != null) Data.isMerging = true;

            _collider.enabled = false;
            _rb.velocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic; 
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (Data == null || Data.isMerging) return;

            if (!other.gameObject.CompareTag("Fruit")) return;
            
            FruitUnit otherFruit = other.gameObject.GetComponent<FruitUnit>();
            if (otherFruit == null || otherFruit.Data == null) return;
            
            if (this.Data.instanceID > otherFruit.Data.instanceID)
            {
                var payload = new FruitCollisionPayload
                {
                    FruitA = this,
                    FruitB = otherFruit
                };
                
                EventManager<FruitMergeEventID>.Post(FruitMergeEventID.FruitCollision, payload);
            }
        }
        
        public int Level => Data?.level ?? -1;
    }
    
}