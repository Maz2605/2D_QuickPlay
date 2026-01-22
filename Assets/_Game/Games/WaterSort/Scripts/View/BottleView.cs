using UnityEngine;
using UnityEngine.Rendering; // Bắt buộc có để dùng SortingGroup
using System.Collections.Generic;
using DG.Tweening; 
using System;      
using _Game.Games.WaterSort.Scripts.Config;
using _Game.Games.WaterSort.Scripts.Model;
using _Game.Games.WaterSort.Scripts.Controller;

namespace _Game.Games.WaterSort.Scripts.View
{
    [RequireComponent(typeof(SortingGroup))] // Tự động thêm SortingGroup nếu quên
    public class BottleView : MonoBehaviour
    {
        [Header("--- HIERARCHY REFS ---")]
        [Tooltip("Object cha chứa toàn bộ hình ảnh (VisualHolder)")]
        [SerializeField] private Transform visualHolder;

        [Tooltip("Sprite Vỏ Chai - Để chỉnh Order đè lên nước")]
        [SerializeField] private SpriteRenderer bottleBodySR; 
        
        [Tooltip("Sprite Mask - Để cắt nước lọt vào trong")]
        [SerializeField] private SpriteMask maskShape; 

        [Tooltip("List các Sprite nước (Trong Liquid_Container)")]
        [SerializeField] public List<SpriteRenderer> liquidRenderers; 

        [Header("--- COMPONENTS ---")]
        [SerializeField] private SortingGroup sortingGroup; 

        [Header("--- EFFECTS ---")]
        [SerializeField] private ParticleSystem successParticles; 

        // --- DATA ---
        private BottleModel _model;
        public BottleModel Model => _model;

        private Vector3 _originalLocalPos;
        private Vector3 _originalScale;
        private float _liquidStandardScaleX = 1.2f;

        private void Start()
        {
            // Cache vị trí gốc
            if (visualHolder != null)
            {
                _originalLocalPos = visualHolder.localPosition;
                _originalScale = visualHolder.localScale;
            }

            // Lấy mẫu scale X từ lớp nước đầu tiên
            if (liquidRenderers.Count > 0 && liquidRenderers[0] != null)
                _liquidStandardScaleX = liquidRenderers[0].transform.localScale.x;

            // Setup SortingGroup
            if (sortingGroup == null) sortingGroup = GetComponent<SortingGroup>();
            
            // Thiết lập thứ tự vẽ nội bộ (Vỏ đè Nước, Nước nằm trong Mask)
            SetupInternalSorting();
        }

        /// <summary>
        /// Cài đặt thứ tự vẽ bên trong cái chai. 
        /// Mask < Nước < Vỏ.
        /// </summary>
        private void SetupInternalSorting()
        {
            // 1. Mask nằm dưới cùng
            if (maskShape) 
            {
                maskShape.backSortingOrder = -1;
                maskShape.frontSortingOrder = 1;
            }

            // 2. Nước nằm giữa (bị Mask cắt)
            foreach (var sr in liquidRenderers)
            {
                if (sr) 
                {
                    sr.sortingOrder = 1; 
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
            }

            // 3. Vỏ chai nằm trên cùng (Che mép nước)
            if (bottleBodySR) 
            {
                bottleBodySR.sortingOrder = 2; 
                bottleBodySR.maskInteraction = SpriteMaskInteraction.None;
            }
        }

        public void Initialize(BottleModel model)
        {
            _model = model;
            UpdateVisuals();
            SetHighPriority(false); // Reset về lớp thường
            
            if(successParticles) successParticles.Stop();
        }

        public void UpdateVisuals()
        {
            if (_model == null || LevelManager.Instance == null) return;
            
            int[] liquids = _model.GetLiquidsForView();
            ColorConfigSO colorConfig = LevelManager.Instance.GetSettings().colorConfig;

            for (int i = 0; i < liquidRenderers.Count; i++)
            {
                if (liquidRenderers[i] == null) continue;

                if (i < liquids.Length)
                {
                    liquidRenderers[i].gameObject.SetActive(true);
                    liquidRenderers[i].color = colorConfig.GetColor(liquids[i]);
                    // Reset scale X về chuẩn, Y=1 (đầy lớp đó)
                    liquidRenderers[i].transform.localScale = new Vector3(_liquidStandardScaleX, 1f, 1f); 
                }
                else
                {
                    liquidRenderers[i].gameObject.SetActive(false);
                }
            }
        }

        // --- HÀNH ĐỘNG CHỌN / BỎ CHỌN ---
        
        /// <summary>
        /// Đưa chai lên lớp hiển thị cao nhất (100) để không bị các chai khác che khuất khi bay.
        /// </summary>
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
                SetHighPriority(true); // Bay lên -> Ưu tiên cao
                visualHolder.DOLocalMoveY(_originalLocalPos.y + 0.5f, animDuration).SetEase(Ease.OutBack);
                visualHolder.DOScale(_originalScale * 1.1f, animDuration).SetEase(Ease.OutBack);
            }
            else
            {
                SetHighPriority(false); // Hạ xuống -> Ưu tiên thấp
                visualHolder.DOLocalMoveY(_originalLocalPos.y, animDuration).SetEase(Ease.OutBounce);
                visualHolder.DOScale(_originalScale, animDuration).SetEase(Ease.OutElastic);
            }
        }

        // --- 🔥 HIỆU ỨNG LẮC KHI SAI (MANUAL SEQUENCE) 🔥 ---
        public void AnimateShakeError(Action onShakeCompleted = null)
        {
            visualHolder.DOKill(); 

            // Hard reset vị trí về trạng thái "Đang chọn" (trên cao)
            visualHolder.localPosition = new Vector3(0, 0.5f, 0); 
            visualHolder.localRotation = Quaternion.identity; 

            // Tạo chuỗi lắc thủ công: Trái -> Phải -> Trái -> Giữa
            Sequence seq = DOTween.Sequence();
            float time = 0.05f; 
            float angle = 15f;  

            seq.Append(visualHolder.DOLocalRotate(new Vector3(0, 0, angle), time));   // Nghiêng Trái
            seq.Append(visualHolder.DOLocalRotate(new Vector3(0, 0, -angle), time*2));// Nghiêng Phải
            seq.Append(visualHolder.DOLocalRotate(new Vector3(0, 0, angle), time*2)); // Nghiêng Trái
            seq.Append(visualHolder.DOLocalRotate(Vector3.zero, time));               // Về giữa

            seq.OnComplete(() => {
                visualHolder.localRotation = Quaternion.identity;
                onShakeCompleted?.Invoke();
            });
        }

        // --- 🔥 HIỆU ỨNG HOÀN THÀNH 🔥 ---
        public void PlaySolvedEffect()
        {
            // Nảy lên 1 cái vui mắt
            visualHolder.DOKill(true);
            visualHolder.DOPunchScale(new Vector3(0.2f, -0.1f, 0), 0.4f, 10, 1);

            // Bắn Particle
            if (successParticles != null)
            {
                successParticles.Play();
            }
        }

        // --- ANIMATION RÓT NƯỚC ---
        public void AnimatePouring(BottleView targetBottle, int amountToTransfer, 
                                   Action<bool> onPourAction, Action onLogicChange, Action onCompleted)
        {
            WaterSortGameConfig settings = LevelManager.Instance.GetSettings();
            if (settings == null) { onCompleted?.Invoke(); return; }

            visualHolder.DOKill();
            
            // Đang bay đi rót -> Ưu tiên cao nhất
            SetHighPriority(true); 

            // Tính toán vị trí & góc
            float direction = (transform.position.x < targetBottle.transform.position.x) ? -1f : 1f;
            float finalOffsetX = Mathf.Abs(settings.pourOffset.x) * direction;
            Vector3 targetPos = targetBottle.transform.position + new Vector3(finalOffsetX, settings.pourOffset.y, 0);
            float finalAngle = (direction == -1f) ? -settings.pourAngle : settings.pourAngle;
            
            float distance = Vector3.Distance(visualHolder.position, targetPos);
            float moveTime = Mathf.Clamp(distance / settings.moveSpeed, 0.25f, 0.5f);
            float rotateTime = 0.3f;

            Sequence seq = DOTween.Sequence();

            // 1. Bay đến
            seq.Append(visualHolder.DOMove(targetPos, moveTime).SetEase(Ease.OutCubic));
            seq.Join(visualHolder.DOScale(_originalScale, moveTime)); 

            // 2. Nghiêng chai -> Callback bắt đầu rót (âm thanh)
            seq.Append(visualHolder.DORotate(new Vector3(0, 0, finalAngle), rotateTime).SetEase(Ease.OutBack));
            seq.AppendCallback(() => onPourAction?.Invoke(true)); 

            // 3. Hiệu ứng chảy nước (Tween Scale)
            List<SpriteRenderer> sourceRenderers = new List<SpriteRenderer>();
            foreach (var r in liquidRenderers) if (r.gameObject.activeSelf) sourceRenderers.Add(r);
            
            List<SpriteRenderer> targetAllRenderers = targetBottle.liquidRenderers;
            int targetStartIndex = 0;
            for(int i=0; i<targetAllRenderers.Count; i++) if(targetAllRenderers[i].gameObject.activeSelf) targetStartIndex++;

            Color liquidColor = settings.colorConfig.GetColor(_model.TopColor);

            for (int i = 0; i < amountToTransfer; i++)
            {
                int sourceIndex = sourceRenderers.Count - 1 - i; 
                int targetIndex = targetStartIndex + i;          

                if (sourceIndex >= 0 && targetIndex < targetAllRenderers.Count)
                {
                    SpriteRenderer sRend = sourceRenderers[sourceIndex];
                    SpriteRenderer tRend = targetAllRenderers[targetIndex];

                    // Setup đích
                    tRend.gameObject.SetActive(true);
                    tRend.color = liquidColor;
                    tRend.transform.localScale = new Vector3(_liquidStandardScaleX, 0f, 1f); 
                    
                    // Tween song song: Nguồn giảm -> Đích tăng
                    seq.Append(sRend.transform.DOScaleY(0f, settings.timePerLayer).SetEase(Ease.Linear)); 
                    seq.Join(tRend.transform.DOScaleY(1f, settings.timePerLayer).SetEase(Ease.Linear));
                    
                    seq.AppendCallback(() => sRend.gameObject.SetActive(false));
                }
            }

            // 4. Kết thúc rót -> Update Data & Dừng âm thanh
            seq.AppendCallback(() => onLogicChange?.Invoke());
            seq.AppendCallback(() => onPourAction?.Invoke(false)); 

            // 5. Quay về
            seq.Append(visualHolder.DORotate(Vector3.zero, rotateTime).SetEase(Ease.InSine));
            float returnTime = Mathf.Clamp(Vector3.Distance(targetPos, transform.TransformPoint(_originalLocalPos)) / settings.moveSpeed, 0.25f, 0.5f);
            seq.Append(visualHolder.DOLocalMove(_originalLocalPos, returnTime).SetEase(Ease.OutSine));

            seq.OnComplete(() => {
                SetHighPriority(false); // Trả về lớp thường
                onCompleted?.Invoke();
            });
        }
    }
}