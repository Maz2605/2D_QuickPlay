using UnityEngine;
using System.Collections.Generic;
using _Game.Core.Scripts.Utils.DesignPattern.Singleton;
using _Game.Games.WaterSort.Scripts.Config;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public class LevelManager : Singleton<LevelManager>
    {
        [Header("--- GLOBAL CONFIG ---")]
        [SerializeField] private WaterSortGameConfig gameSetting;

        [Header("--- LEVEL DATA ---")]
        [SerializeField] private List<LevelConfigSO> allLevels;

        [Header("--- REFS ---")]
        [SerializeField] private WaterSortController gamePlayController;

        private int _currentLevelIndex = 0;

        protected override void Awake()
        {
            base.Awake(); 
            if (Instance != null && Instance != this) return;

            if (gameSetting != null)
                WaterSortDataManager.Instance.Initialize(gameSetting.gameID);
        }

        private void Start()
        {
            _currentLevelIndex = WaterSortDataManager.Instance.GetCurrentLevel();
            LoadCurrentLevel();
        }

        public void LoadCurrentLevel()
        {
            if (allLevels == null || allLevels.Count == 0) return;
            if (_currentLevelIndex >= allLevels.Count) _currentLevelIndex = 0;
        
            Debug.Log($"[LevelManager] Loading Level: {_currentLevelIndex}");

            // --- KIỂM TRA TIẾN TRÌNH CŨ ---
            if (WaterSortDataManager.Instance.HasProgressInCurrentLevel())
            {
                // Sau này bạn sẽ gọi Popup Resume ở đây
                Debug.LogWarning($"[POPUP] Phát hiện màn {_currentLevelIndex} đang chơi dở! Bạn có muốn chơi tiếp không?");
            }
            
            if (gamePlayController != null)
                gamePlayController.LoadLevel(allLevels[_currentLevelIndex]);
        }

        // --- HÀM REPLAY (CHƠI LẠI MÀN HIỆN TẠI TỪ ĐẦU) ---
        public void OnClickReplayLevel()
        {
            Debug.Log("[LevelManager] Replaying current level...");
            WaterSortDataManager.Instance.ClearLevelProgress();

            LoadCurrentLevel();
        }

        public void OnLevelWin()
        {
            Debug.Log("Victory Logic!");
            WaterSortDataManager.Instance.UnlockNextLevel();
            _currentLevelIndex = WaterSortDataManager.Instance.GetCurrentLevel();
        }

        // Hàm này reset toàn bộ game về Level 0 (Hard Reset)
        public void OnClickResetGame()
        {
            WaterSortDataManager.Instance.ResetProgress();
            _currentLevelIndex = WaterSortDataManager.Instance.GetCurrentLevel();
            LoadCurrentLevel();
        }
        
        public void LoadNextLevel()
        {
            LoadCurrentLevel();
        }

        public string GetCurrentLevelName()
        {
            if (allLevels != null && _currentLevelIndex < allLevels.Count)
            {
                string customName = allLevels[_currentLevelIndex].nameLevel;
                return string.IsNullOrEmpty(customName) ? $"Level {_currentLevelIndex + 1}" : customName;
            }
            return $"Level {_currentLevelIndex + 1}";
        }
    
        public WaterSortGameConfig GetSettings() => gameSetting;
    }
}