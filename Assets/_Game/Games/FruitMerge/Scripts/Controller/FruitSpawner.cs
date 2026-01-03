using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Core.Scripts.Input;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.View;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Random = System.Random;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public class FruitSpawner : MonoBehaviour
    {
        [Header("Settings")] 
        // [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float xLimit = 2.5f;
        [SerializeField] private Transform dropPoint;
        [SerializeField] private SpriteRenderer currentRenderer;
        [SerializeField] private NextFruitPanel nextFruitPanel; 

        [Header("Danger Zone")] 
        [SerializeField] private Transform dangerLine;
        [SerializeField] private float dangerCheckHeight = 2.0f;
        [SerializeField] private float dangerCheckWidth = 6.0f;
        [SerializeField] private LayerMask fruitLayer;
        
        private FruitMergeGameConfigSO _config;
        private Action<int, Vector3> _spawnCallback;
        
        private List<int> _queue = new List<int>(); 

        private int _nextLevelToSpawn;
        private bool _canSpawn = true;
        private float _currentX;

        public void Initialize(FruitMergeGameConfigSO config, Action<int, Vector3> onSpawnRequest)
        {
            _config = config;
            _spawnCallback = onSpawnRequest;
            _currentX = transform.position.x;

            for (int i = 0; i < 4; i++)
            {
                _queue.Add(_config.GetSmartSpawnLevel(false,-1));
            }
            

            InputManager.Instance.OnTouchMove += HandleMove;
            InputManager.Instance.OnTouchEnd += HandleRelease;
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTouchMove -= HandleMove;
                InputManager.Instance.OnTouchEnd -= HandleRelease;
            }
        }

        private void HandleMove(Vector2 screenPos)
        {
            if(!_canSpawn) return;

            float worldX = InputManager.Instance.GetWorldPosition().x;
            float clampedX = Mathf.Clamp(worldX, -xLimit, xLimit);
            currentRenderer.transform.position = new Vector3(clampedX, dropPoint.position.y, 0);
        }

        private void HandleRelease(Vector2 screenPos)
        {
            if(!_canSpawn) return;
            
            HandleMove(screenPos);
            StartCoroutine(SpawnRoutine(currentRenderer.transform.position));
        }

        private IEnumerator SpawnRoutine(Vector3 position)
        {
            _canSpawn = false;
            currentRenderer.enabled = false;

            int levelToSpawn = _queue[0];
            _spawnCallback?.Invoke(levelToSpawn, position);

            yield return new WaitForSeconds(_config.spawnCooldown);

            AdvanceQueue();
            _canSpawn = true;
            currentRenderer.enabled = true;
        }

        private void AdvanceQueue()
        {
            if(_queue.Count > 0) _queue.RemoveAt(0);

            bool isDanger = CheckDanger();
            int lastLevel = (_queue.Count > 0) ? _queue[_queue.Count - 1] : -1;
            
            _queue.Add(_config.GetSmartSpawnLevel(isDanger, lastLevel));
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if(_queue.Count == 0) return;

            var curInfo = _config.GetInfo(_queue[0]);
            currentRenderer.sprite = curInfo.visual;
            currentRenderer.transform.localScale = Vector3.one * curInfo.scale;

            List<Sprite> previews = new List<Sprite>();
            for (int i = 1; i < _queue.Count; i++)
            {
                var info = _config.GetInfo(_queue[i]);
                if(info != null) previews.Add(info.visual);
            }
            nextFruitPanel.UpdatePreview(previews);
        }

        private bool CheckDanger()
        {
            if (dangerLine == null) return false;

            Vector2 center = (Vector2)dangerLine.position - new Vector2(0, dangerCheckHeight / 2f);
            return Physics2D.OverlapBox(center, new Vector2(dangerCheckWidth, dangerCheckHeight), 0, fruitLayer);
        }

        private void OnDrawGizmos()
        {
            if (dangerLine == null) return;

            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Vector2 center =(Vector2) dangerLine.position - new Vector2(0, dangerCheckHeight / 2f);
            Gizmos.DrawCube(center, new Vector2(dangerCheckWidth, dangerCheckHeight));
        }
    }
}