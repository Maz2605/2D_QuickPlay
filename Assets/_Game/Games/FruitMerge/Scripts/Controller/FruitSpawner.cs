using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Core.Scripts.Input;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using UnityEngine;
using DG.Tweening; 
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.View;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    [RequireComponent(typeof(LineRenderer))]
    public class FruitSpawner : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private float xLimit = 2.5f;
        [SerializeField] private Transform dropPoint;
        [SerializeField] private SpriteRenderer currentRenderer;

        [Header("Juicy & Guide")]
        [SerializeField] private float dragScaleFactor = 1.1f; 
        [SerializeField] private float animDuration = 0.2f;
        [SerializeField] private LineRenderer aimLine; 
        [SerializeField] private LayerMask fruitLayer; 
        
        [Header("Danger Zone")] 
        [SerializeField] private Transform dangerLine;
        [SerializeField] private float dangerCheckHeight = 2.0f;
        [SerializeField] private float dangerCheckWidth = 6.0f;
        
        private FruitMergeGameConfigSO _config;
        private Action<int, Vector3> _spawnCallback;
        
        private List<int> _queue = new List<int>(); 
        private bool _canSpawn = true;
        private float _baseScale = 1f;
        private bool _isInputLocked = false; 

        public void Initialize(FruitMergeGameConfigSO config, Action<int, Vector3> onSpawnRequest)
        {
            _config = config;
            _spawnCallback = onSpawnRequest;

            if (aimLine == null) aimLine = GetComponent<LineRenderer>();
            aimLine.positionCount = 2;
            aimLine.enabled = false; 
            
            ResetDataAndUI();
        }

        private void OnDestroy()
        {
            transform.DOKill();
            currentRenderer.transform.DOKill();
        }

        public void SetInputActive(bool isActive)
        {
            _isInputLocked = !isActive;
            if (_isInputLocked)
            {
                aimLine.enabled = false;
                currentRenderer.transform.DOKill();
                currentRenderer.transform.localScale = Vector3.one * _baseScale;
            }
        }

        public void ResetSpawner()
        {
            StopAllCoroutines();
            currentRenderer.transform.DOKill();
            transform.DOKill();
            
            _canSpawn = true;
            _isInputLocked = false;
            aimLine.enabled = false;

            if (dropPoint != null)
                currentRenderer.transform.position = dropPoint.position;
            
            EventManager<FruitMergeEventID>.Post(FruitMergeEventID.NextFruitChanged, new List<Sprite>());

            ResetDataAndUI();
        }
        
        private void ResetDataAndUI()
        {
            _queue.Clear();
            for (int i = 0; i < 4; i++)
            {
                _queue.Add(_config.GetSmartSpawnLevel(false, -1));
            }
            ShowNextFruit(true);
        }

        // --- INPUT HANDLING (Được gọi từ Controller) ---
        
        public void HandleStart(Vector2 screenPos)
        {
            if (!_canSpawn || _isInputLocked) return;
            
            MoveFruit(screenPos);

            aimLine.enabled = true;
            currentRenderer.transform.DOKill();
            currentRenderer.transform.DOScale(_baseScale * dragScaleFactor, animDuration).SetEase(Ease.InBack);
        }

        public void HandleMove(Vector2 screenPos)
        {
            if (!_canSpawn || _isInputLocked) return;
            MoveFruit(screenPos);
        }

        public void HandleRelease(Vector2 screenPos)
        {
            if (!_canSpawn || _isInputLocked) return;
            
            MoveFruit(screenPos);
            aimLine.enabled = false; 

            StartCoroutine(SpawnRoutine(currentRenderer.transform.position));
        }

        private void MoveFruit(Vector2 screenPos)
        {
            float worldX = InputManager.Instance.GetWorldPosition().x;
            
            float clampedX = Mathf.Clamp(worldX, -xLimit, xLimit);
            currentRenderer.transform.position = new Vector3(clampedX, dropPoint.position.y, 0);
        }

        // --- SPAWN LOGIC (Giữ nguyên) ---
        private IEnumerator SpawnRoutine(Vector3 position)
        {
            _canSpawn = false;
            currentRenderer.transform.DOKill();
            currentRenderer.enabled = false;

            int levelToSpawn = _queue[0];
            _spawnCallback?.Invoke(levelToSpawn, position);

            yield return new WaitForSeconds(_config.spawnCooldown);

            AdvanceQueue();
            ShowNextFruit();
            
            _canSpawn = true;
        }

        private void AdvanceQueue()
        {
            if (_queue.Count > 0) _queue.RemoveAt(0);
            bool isDanger = CheckDanger();
            int lastLevel = (_queue.Count > 0) ? _queue[_queue.Count - 1] : -1;
            _queue.Add(_config.GetSmartSpawnLevel(isDanger, lastLevel));
        }

        private void ShowNextFruit(bool isInit = false)
        {
            if (_queue.Count == 0) return;

            var curInfo = _config.GetInfo(_queue[0]);
            _baseScale = curInfo.scale;

            currentRenderer.sprite = curInfo.visual;
            currentRenderer.enabled = true;

            currentRenderer.transform.localScale = Vector3.zero;
            currentRenderer.transform.DOScale(_baseScale, animDuration)
                .SetEase(Ease.OutBack)
                .SetLink(currentRenderer.gameObject);

            List<Sprite> previews = new List<Sprite>();
            for (int i = 1; i < _queue.Count; i++)
            {
                var info = _config.GetInfo(_queue[i]);
                if (info != null) previews.Add(info.visual);
            }

            EventManager<FruitMergeEventID>.Post(FruitMergeEventID.NextFruitChanged, previews);
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
            Vector2 center = (Vector2)dangerLine.position - new Vector2(0, dangerCheckHeight / 2f);
            Gizmos.DrawCube(center, new Vector2(dangerCheckWidth, dangerCheckHeight));
        }
    }
}