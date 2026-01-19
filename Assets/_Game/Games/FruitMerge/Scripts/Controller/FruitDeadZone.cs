using UnityEngine;
using System;
using _Game.Games.FruitMerge.Scripts.View;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public class FruitDeadZone : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float maxTimeOverLine = 5.0f;
        [Tooltip("Vận tốc nhỏ hơn mức này mới tính là đang đọng lại ở vạch")]
        [SerializeField] private float velocityThreshold = 0.5f;
        
        public event Action<float> OnDangerTimerUpdate; 
        public event Action OnGameOverTriggered;

        private float _timer = 0f;
        private bool _isObjectInZone = false;
        private Rigidbody2D _targetRb;

        private void Update()
        {
            if (!_isObjectInZone || _targetRb == null) return;
            
            if (_targetRb.velocity.sqrMagnitude < velocityThreshold)
            {
                _timer += Time.deltaTime;
                
                OnDangerTimerUpdate?.Invoke(_timer / maxTimeOverLine);

                if (_timer >= maxTimeOverLine)
                {
                    OnGameOverTriggered?.Invoke();
                    ResetZone();
                }
            }
            else
            {
                ResetTimer();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isObjectInZone && _targetRb != null) return;

            if (other.TryGetComponent(out FruitUnit fruit)) 
            {
                _targetRb = other.attachedRigidbody;
                _isObjectInZone = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_isObjectInZone && other.attachedRigidbody == _targetRb)
            {
                ResetZone();
            }
        }

        public void ResetZone()
        {
            _isObjectInZone = false;
            _targetRb = null;
            ResetTimer();
        }

        private void ResetTimer()
        {
            _timer = 0f;
            OnDangerTimerUpdate?.Invoke(0f);
        }
    }
}