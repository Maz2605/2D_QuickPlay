using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Nhớ có dòng này
using _Game.Core.Scripts.Input;
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
        [SerializeField] private NextFruitPanel nextFruitPanel; 

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

        public void Initialize(FruitMergeGameConfigSO config, Action<int, Vector3> onSpawnRequest)
        {
            _config = config;
            _spawnCallback = onSpawnRequest;

            if (aimLine == null) aimLine = GetComponent<LineRenderer>();
            aimLine.positionCount = 2;
            aimLine.enabled = false; 

            for (int i = 0; i < 4; i++)
            {
                _queue.Add(_config.GetSmartSpawnLevel(false, -1));
            }

            InputManager.Instance.OnTouchStart += HandleStart;
            InputManager.Instance.OnTouchMove += HandleMove;
            InputManager.Instance.OnTouchEnd += HandleRelease;

            ShowNextFruit(true);
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTouchStart -= HandleStart;
                InputManager.Instance.OnTouchMove -= HandleMove;
                InputManager.Instance.OnTouchEnd -= HandleRelease;
            }
            transform.DOKill();
            currentRenderer.transform.DOKill();
        }

        // --- INPUT HANDLING ---

        private void HandleStart(Vector2 screenPos)
        {
            if (!_canSpawn) return;
            
            MoveFruit(screenPos);

            aimLine.enabled = true;
            // DrawAimLine(currentRenderer.transform.position);

            currentRenderer.transform.DOKill();
            currentRenderer.transform.DOScale(_baseScale * dragScaleFactor, animDuration).SetEase(Ease.InBack);
        }

        private void HandleMove(Vector2 screenPos)
        {
            if (!_canSpawn) return;
            MoveFruit(screenPos);
            // DrawAimLine(currentRenderer.transform.position);
        }

        private void HandleRelease(Vector2 screenPos)
        {
            if (!_canSpawn) return;
            
            MoveFruit(screenPos);
            aimLine.enabled = false; // Tắt tia ngắm

            StartCoroutine(SpawnRoutine(currentRenderer.transform.position));
        }

        private void MoveFruit(Vector2 screenPos)
        {
            float worldX = InputManager.Instance.GetWorldPosition().x;
            float clampedX = Mathf.Clamp(worldX, -xLimit, xLimit);
            currentRenderer.transform.position = new Vector3(clampedX, dropPoint.position.y, 0);
        }

        // --- SPAWN LOGIC ---

        private IEnumerator SpawnRoutine(Vector3 position)
        {
            _canSpawn = false;

            currentRenderer.transform.DOKill();

            currentRenderer.enabled = false;

            int levelToSpawn = _queue[0];
            _spawnCallback?.Invoke(levelToSpawn, position);

            yield return new WaitForSeconds(_config.spawnCooldown);

            yield return currentRenderer.transform.DOScale(0f, 0.15f).SetEase(Ease.InBack).WaitForCompletion();
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

        // --- VISUAL LOGIC ---

        private void ShowNextFruit(bool isInit = false)
        {
            if (_queue.Count == 0) return;
            
            var curInfo = _config.GetInfo(_queue[0]);
            _baseScale = curInfo.scale; // Lưu scale gốc

            currentRenderer.sprite = curInfo.visual;
            currentRenderer.enabled = true;
            
            currentRenderer.transform.localScale = Vector3.zero;
            currentRenderer.transform.DOScale(_baseScale, animDuration).SetEase(Ease.OutBounce);
            
            List<Sprite> previews = new List<Sprite>();
            for (int i = 1; i < _queue.Count; i++)
            {
                var info = _config.GetInfo(_queue[i]);
                if (info != null) previews.Add(info.visual);
            }
            nextFruitPanel.UpdatePreview(previews);
        }

        private void DrawAimLine(Vector3 startPos)
        {
            aimLine.SetPosition(0, startPos);

            RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, 10f, fruitLayer);
            
            Vector3 endPos;
            if (hit.collider != null)
            {
                endPos = hit.point;
            }
            else
            {
                endPos = startPos + Vector3.down * 10f;
            }
            aimLine.SetPosition(1, endPos);
        }

        // --- GIZMOS ---
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