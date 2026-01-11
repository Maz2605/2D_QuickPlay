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

                // Giả vờ load mất 2 giây (Dùng DOTween delay cho tiện)
                DOVirtual.DelayedCall(2f, () =>
                {
                    Debug.Log("3. Load xong! Gọi Hide Loading.");
                    UIManager.Instance.HideLoading();
                });
            });
        }
        
    }
}