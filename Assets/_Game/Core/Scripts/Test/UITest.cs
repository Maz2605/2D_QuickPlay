using _Game.Core.Scripts.UI.Manager;
using DG.Tweening;
using UnityEngine;

namespace _Game.Core.Scripts.Test
{
    public class UITest : MonoBehaviour
    {
        public void TestNormalToast()
        {
            UIManager.Instance.ShowToast("Lưu game thành công!");
        }

        public void TestErrorToast()
        {
            UIManager.Instance.ShowToast("Lỗi kết nối! Vui lòng kiểm tra Wifi.", 4f);
        }

        public void TestSpamToast()
        {
            int range = Random.Range(0, 100);
            for (int i = 0; i < range; i++)
            {
                UIManager.Instance.ShowToast("Spam: " + i);
            }
        }

        public void TestLoadingScreen()
        {
            UIManager.Instance.ShowLoading(
                () =>
                {
                    UIManager.Instance.ShowToast("Loading...");
                });
        }
        public void Test_SimulateFakeLoad()
        {
            Debug.Log("1. Bắt đầu gọi Loading...");
            
            UIManager.Instance.ShowLoading(() =>
            {
                // Callback này chạy khi màn hình đã tối thui
                Debug.Log("2. Màn hình đã tối đen. Đang tải dữ liệu giả...");

                DOVirtual.DelayedCall(2f, () =>
                {
                    Debug.Log("3. Load xong! Gọi Hide Loading.");
                    UIManager.Instance.HideLoading();
                });
            });
        }
        
        // [Header("--- CONFIRMATION TESTS ---")]

        // Kịch bản 1: Mua vật phẩm
        [ContextMenu("Test Confirm: Mua Đồ")]
        public void Test_BuyItem()
        {
            UIManager.Instance.ShowConfirmation(
                "XÁC NHẬN MUA",                         
                "Bạn có muốn dùng 500 Vàng để mua Kiếm Thần không?", 
                () => 
                {
                    Debug.Log(">>> Đã bấm YES: Trừ 500 vàng. Cộng 1 Kiếm.");
                    UIManager.Instance.ShowToast("Mua thành công!");
                },
                () => 
                {
                    Debug.Log(">>> Đã bấm NO: Hủy giao dịch.");
                },
                "MUA NGAY",
                "ĐỂ SAU"
            );
        }

        // Kịch bản 2: Xóa dữ liệu (Hành động nguy hiểm)
        [ContextMenu("Test Confirm: Xóa Save")]
        public void Test_DeleteSave()
        {
            UIManager.Instance.ShowConfirmation(
                "CẢNH BÁO",
                "Hành động này không thể hoàn tác. Bạn có chắc chắn muốn xóa file save?",
                OnDeleteConfirmed, // Có thể truyền tên hàm thay vì viết lambda () => {}
                OnDeleteCancelled
            );
        }

        // Viết hàm rời cho gọn nếu logic dài
        private void OnDeleteConfirmed()
        {
            Debug.Log(">>> Đang xóa file save...");
            UIManager.Instance.ShowLoading(() => 
            {
                // Giả vờ xóa mất 2 giây
                DOVirtual.DelayedCall(2f, () => 
                {
                    UIManager.Instance.HideLoading();
                    UIManager.Instance.ShowToast("Đã xóa dữ liệu!");
                });
            });
        }

        private void OnDeleteCancelled()
        {
            Debug.Log(">>> May quá chưa xóa!");
        }
    }
    
}