using UnityEngine;
using System.Collections.Generic;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.WaterSort.Scripts.Config;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    // [THAY ĐỔI] Bỏ kế thừa Singleton, chỉ là MonoBehaviour thường
    [DefaultExecutionOrder(-5)] 
    public class LevelManager : MonoBehaviour
    {
        [Header("--- GLOBAL CONFIG ---")]
        [SerializeField] private WaterSortGameConfig gameSetting;

        [Header("--- LEVEL DATA ---")]
        [SerializeField] private List<LevelConfigSO> allLevels;

        private WaterSortController _activeController;
        private WaterSortDataManager _dataManager;
        private int _currentLevelIndex = 0;

        private void Awake()
        {
            if (gameSetting != null && !string.IsNullOrEmpty(gameSetting.gameID))
            {
                _dataManager = new WaterSortDataManager(gameSetting.gameID);
            }
            else
            {
                Debug.LogError("[LevelManager] GameConfig is NULL or GameID empty! Fallback to default.");
                _dataManager = new WaterSortDataManager("default_water_sort");
            }
            
            _currentLevelIndex = _dataManager.GetCurrentLevel();
        }

        private void OnEnable()
        {
            EventManager<WaterSortEventID>.AddListener(WaterSortEventID.LevelWin, OnLevelWinReceived);
        }

        private void OnDisable()
        {
            EventManager<WaterSortEventID>.RemoveListener(WaterSortEventID.LevelWin, OnLevelWinReceived);
        }

        // --- HÀM BẮT TAY (HANDSHAKE) ---
        public void InitGame(WaterSortController controller)
        {
            _activeController = controller;
            
            // Cập nhật lại index mới nhất
            _currentLevelIndex = _dataManager.GetCurrentLevel();
            
            // Bắt đầu load level
            LoadCurrentLevel();
        }

        public void LoadCurrentLevel()
        {
            if (allLevels == null || allLevels.Count == 0)
            {
                Debug.LogError("[LevelManager] Level List is Empty!");
                return;
            }
            
            // Validate Index
            if (_currentLevelIndex >= allLevels.Count || _currentLevelIndex < 0) 
            {
                Debug.LogWarning($"[LevelManager] Invalid Index {_currentLevelIndex}. Resetting to 0.");
                _currentLevelIndex = 0;
                _dataManager.ResetProgress(); 
            }
        
            Debug.Log($"[LevelManager] Loading Level Index: {_currentLevelIndex}");

            if (_dataManager.HasProgressInCurrentLevel())
            {
                // TODO: Show Popup Resume logic
            }
            
            if (_activeController != null)
            {
                _activeController.LoadLevel(allLevels[_currentLevelIndex], _currentLevelIndex);
            }
            
            _dataManager.MarkLevelStarted();
        }

        // --- EVENT HANDLERS ---

        private void OnLevelWinReceived()
        {
            _dataManager.UnlockNextLevel();
            _currentLevelIndex = _dataManager.GetCurrentLevel();
        }

        // --- PUBLIC ACTIONS ---

        public void OnClickReplayLevel()
        {
            _dataManager.ClearLevelProgress();
            LoadCurrentLevel();
        }

        public void OnClickResetGame()
        {
            _dataManager.ResetProgress();
            _currentLevelIndex = 0;
            LoadCurrentLevel();
        }
        
        public void HardDeleteData()
        {
            Debug.LogWarning("[LevelManager] Hard Deleting Data...");
            _dataManager.DeleteSaveFile();
            
            _currentLevelIndex = 0;
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