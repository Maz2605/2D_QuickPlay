using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using DG.Tweening; 
using System;      
using _Game.Games.WaterSort.Scripts.Config;
using _Game.Games.WaterSort.Scripts.Model;

namespace _Game.Games.WaterSort.Scripts.View
{
    [RequireComponent(typeof(SortingGroup))]
    public class BottleView : MonoBehaviour
    {
        [Header("--- HIERARCHY REFS ---")]
        [SerializeField] private Transform visualHolder;
        [SerializeField] private SpriteRenderer bottleBodySR; 
        [SerializeField] private SpriteMask maskShape; 
        [SerializeField] public List<SpriteRenderer> liquidRenderers; 

        [Header("--- COMPONENTS ---")]
        [SerializeField] private SortingGroup sortingGroup; 
        [SerializeField] private ParticleSystem successParticles; 

        // --- DATA ---
        private BottleModel _model;
        private WaterSortGameConfig _config; // [MỚI] Lưu config để dùng cho Animation
        
        public BottleModel Model => _model;

        // Cache biến visual
        private Vector3 _originalLocalPos;
        private Vector3 _originalScale;
        private float _liquidStandardScaleX = 1.2f;

        private void Awake()
        {
            if (sortingGroup == null) sortingGroup = GetComponent<SortingGroup>();
        }

        private void Start()
        {
            if (visualHolder != null)
            {
                _originalLocalPos = visualHolder.localPosition;
                _originalScale = visualHolder.localScale;
            }

            if (liquidRenderers.Count > 0 && liquidRenderers[0] != null)
                _liquidStandardScaleX = liquidRenderers[0].transform.localScale.x;

            SetupInternalSorting();
        }

        private void SetupInternalSorting()
        {
            if (maskShape) 
            {
                maskShape.backSortingOrder = -1;
                maskShape.frontSortingOrder = 1;
            }
            foreach (var sr in liquidRenderers)
            {
                if (sr) 
                {
                    sr.sortingOrder = 1; 
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
            }
            if (bottleBodySR) 
            {
                bottleBodySR.sortingOrder = 2; 
                bottleBodySR.maskInteraction = SpriteMaskInteraction.None;
            }
        }

        public void Initialize(BottleModel model, WaterSortGameConfig config)
        {
            _model = model;
            _config = config; 
            
            UpdateVisuals();
            SetHighPriority(false); 
            
            if(successParticles) successParticles.Stop();
        }

        public void UpdateVisuals()
        {
            if (_model == null || _config == null) return;
            
            int[] liquids = _model.GetLiquidsForView();
            ColorConfigSO colorConfig = _config.colorConfig; // Lấy màu từ config đã cache

            for (int i = 0; i < liquidRenderers.Count; i++)
            {
                if (liquidRenderers[i] == null) continue;

                if (i < liquids.Length)
                {
                    liquidRenderers[i].gameObject.SetActive(true);
                    liquidRenderers[i].color = colorConfig.GetColor(liquids[i]);
                    liquidRenderers[i].transform.localScale = new Vector3(_liquidStandardScaleX, 1f, 1f); 
                }
                else
                {
                    liquidRenderers[i].gameObject.SetActive(false);
                }
            }
        }

        // --- INTERACTION VISUALS ---
        
        private void SetHighPriority(bool isHigh)
        {
            if (sortingGroup) sortingGroup.sortingOrder = isHigh ? 100 : 0;
        }

        public void SetSelected(bool isSelected)
        {
            if (visualHolder == null) return;
            visualHolder.DOKill();
            
            float animDuration = 0.3f; 

            if (isSelected)
            {
                SetHighPriority(true);
                visualHolder.DOLocalMoveY(_originalLocalPos.y + 0.5f, animDuration).SetEase(Ease.OutBack);
                visualHolder.DOScale(_originalScale * 1.1f, animDuration).SetEase(Ease.OutBack);
            }
            else
            {
                SetHighPriority(false);
                visualHolder.DOLocalMoveY(_originalLocalPos.y, animDuration).SetEase(Ease.OutBounce);
                visualHolder.DOScale(_originalScale, animDuration).SetEase(Ease.OutElastic);
            }
        }

        public void AnimateShakeError(Action onShakeCompleted = null)
        {
            visualHolder.DOKill(); 
            visualHolder.localPosition = new Vector3(0, 0.5f, 0); 
            visualHolder.localRotation = Quaternion.identity; 

            Sequence seq = DOTween.Sequence();
            float time = 0.05f; 
            float angle = 15f;  

            seq.Append(visualHolder.DOLocalRotate(new Vector3(0, 0, angle), time));
            seq.Append(visualHolder.DOLocalRotate(new Vector3(0, 0, -angle), time*2));
            seq.Append(visualHolder.DOLocalRotate(new Vector3(0, 0, angle), time*2));
            seq.Append(visualHolder.DOLocalRotate(Vector3.zero, time));

            seq.OnComplete(() => {
                visualHolder.localRotation = Quaternion.identity;
                onShakeCompleted?.Invoke();
            });
        }

        public void PlaySolvedEffect()
        {
            visualHolder.DOKill(true);
            visualHolder.DOPunchScale(new Vector3(0.2f, -0.1f, 0), 0.4f, 10, 1);
            if (successParticles != null) successParticles.Play();
        }

        // --- CORE ANIMATION: POURING ---
        public void AnimatePouring(BottleView targetBottle, int amountToTransfer, 
                                   Action<bool> onPourAction, Action onLogicChange, Action onCompleted)
        {
            if (_config == null) { onCompleted?.Invoke(); return; }

            visualHolder.DOKill();
            SetHighPriority(true); 

            // Tính toán vị trí dựa trên Config
            float direction = (transform.position.x < targetBottle.transform.position.x) ? -1f : 1f;
            float finalOffsetX = Mathf.Abs(_config.pourOffset.x) * direction;
            Vector3 targetPos = targetBottle.transform.position + new Vector3(finalOffsetX, _config.pourOffset.y, 0);
            float finalAngle = (direction == -1f) ? -_config.pourAngle : _config.pourAngle;
            
            float distance = Vector3.Distance(visualHolder.position, targetPos);
            float moveTime = Mathf.Clamp(distance / _config.moveSpeed, 0.25f, 0.5f);
            float rotateTime = 0.3f;

            Sequence seq = DOTween.Sequence();

            // 1. Bay đến
            seq.Append(visualHolder.DOMove(targetPos, moveTime).SetEase(Ease.OutCubic));
            seq.Join(visualHolder.DOScale(_originalScale, moveTime)); 

            // 2. Nghiêng chai -> Gọi Callback báo "Bắt đầu rót" (Controller sẽ bắn Event Play Sound ở đây)
            seq.Append(visualHolder.DORotate(new Vector3(0, 0, finalAngle), rotateTime).SetEase(Ease.OutBack));
            // seq.AppendCallback(() => onPourAction?.Invoke(true)); 

            // 3. Chảy nước
            List<SpriteRenderer> sourceRenderers = new List<SpriteRenderer>();
            foreach (var r in liquidRenderers) if (r.gameObject.activeSelf) sourceRenderers.Add(r);
            
            List<SpriteRenderer> targetAllRenderers = targetBottle.liquidRenderers;
            int targetStartIndex = 0;
            for(int i=0; i<targetAllRenderers.Count; i++) if(targetAllRenderers[i].gameObject.activeSelf) targetStartIndex++;

            Color liquidColor = _config.colorConfig.GetColor(_model.TopColor);

            for (int i = 0; i < amountToTransfer; i++)
            {
                int sourceIndex = sourceRenderers.Count - 1 - i; 
                int targetIndex = targetStartIndex + i;          

                if (sourceIndex >= 0 && targetIndex < targetAllRenderers.Count)
                {
                    SpriteRenderer sRend = sourceRenderers[sourceIndex];
                    SpriteRenderer tRend = targetAllRenderers[targetIndex];

                    tRend.gameObject.SetActive(true);
                    tRend.color = liquidColor;
                    tRend.transform.localScale = new Vector3(_liquidStandardScaleX, 0f, 1f); 
                    
                    seq.Append(sRend.transform.DOScaleY(0f, _config.timePerLayer).SetEase(Ease.Linear)); 
                    seq.Join(tRend.transform.DOScaleY(1f, _config.timePerLayer).SetEase(Ease.Linear));
                    seq.AppendCallback(() => sRend.gameObject.SetActive(false));
                }
            }

            // 4. Update Model (Logic Change) & Stop Sound
            seq.AppendCallback(() => onLogicChange?.Invoke());
            seq.AppendCallback(() => onPourAction?.Invoke(false)); 

            // 5. Quay về
            seq.Append(visualHolder.DORotate(Vector3.zero, rotateTime).SetEase(Ease.InSine));
            float returnTime = Mathf.Clamp(Vector3.Distance(targetPos, transform.TransformPoint(_originalLocalPos)) / _config.moveSpeed, 0.25f, 0.5f);
            seq.Append(visualHolder.DOLocalMove(_originalLocalPos, returnTime).SetEase(Ease.OutSine));

            seq.OnComplete(() => {
                SetHighPriority(false);
                onCompleted?.Invoke();
            });
        }
    }
}