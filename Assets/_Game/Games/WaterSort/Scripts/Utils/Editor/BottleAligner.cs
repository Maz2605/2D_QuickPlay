using UnityEngine;

namespace _Game.Games.WaterSort.Scripts.Utils.Editor
{
    [ExecuteInEditMode] // Script chạy ngay cả khi không bấm Play
    public class BottleAligner : MonoBehaviour
    {
        [Header("Settings")]
        public float fillRatio = 0.9f; // Nước chiếm bao nhiêu % chiều cao mask (để chừa miệng chai)
        public float liquidWidth = 0.6f; // Chiều ngang cục nước

        [Header("References")]
        public SpriteMask bottleMask;
        public Transform[] liquids; // Kéo Liquid_0 -> Liquid_3 vào đây

        [Header("Actions")]
        public bool autoAlign = false; // Tích vào đây để nó tự căn liên tục

        void Update()
        {
            if (autoAlign)
            {
                AlignLiquids();
            }
        }

        [ContextMenu("Align Now")] // Thêm nút bấm vào menu chuột phải
        public void AlignLiquids()
        {
            if (bottleMask == null || liquids == null || liquids.Length == 0) return;

            // 1. Lấy chiều cao của Mask (Vùng hiển thị cho phép)
            float maskHeight = bottleMask.bounds.size.y * fillRatio;
            float maskBottomY = bottleMask.bounds.min.y;
        
            // Căn chỉnh lại bottom một chút để không bị lẹm quá
            // (Tùy chỉnh offset này dựa trên hình dáng đáy chai thực tế của bạn)
            float bottomOffset = (bottleMask.bounds.size.y * (1 - fillRatio)) / 2; 

            // 2. Tính chiều cao mỗi cục nước
            float segmentHeight = maskHeight / liquids.Length;

            for (int i = 0; i < liquids.Length; i++)
            {
                Transform t = liquids[i];
            
                // --- XỬ LÝ SCALE ---
                // Mặc định Sprite hình vuông 1x1 unit. Scale Y = chiều cao mong muốn.
                // Scale X = chiều rộng mong muốn.
                t.localScale = new Vector3(liquidWidth, segmentHeight, 1f);

                // --- XỬ LÝ VỊ TRÍ (POSITION) ---
                // Mẹo: Sprite mặc định Pivot ở giữa (Center).
                // Vị trí Y = Đáy Mask + Offset + (Thứ tự * Chiều cao) + (Nửa chiều cao chính nó để bù Pivot)
                float posY = maskBottomY + bottomOffset + (i * segmentHeight) + (segmentHeight / 2);
            
                // Giữ nguyên X của mask, chỉ thay đổi Y
                t.position = new Vector3(bottleMask.transform.position.x, posY, t.position.z);
            }
        }
    }
}