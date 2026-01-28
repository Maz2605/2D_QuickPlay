using _Game.Core.Scripts.Data;
using _Game.Games.WaterSort.Scripts.Config;
using _Game.Games.WaterSort.Scripts.Model; 
using UnityEngine;

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public class WaterSortDataManager
    {
        private WaterSortUserData _userData;
        private readonly string _gameID;

        public WaterSortDataManager(string gameID)
        {
            _gameID = gameID;
            LoadData();
        }

        private void LoadData()
        {
            if (string.IsNullOrEmpty(_gameID)) return;
            
            _userData = SaveSystem.Load<WaterSortUserData>(_gameID);
            
            if (_userData == null)
            {
                _userData = new WaterSortUserData();
                Save(); 
            }
        }

        public void Save()
        {
            if (_userData != null && !string.IsNullOrEmpty(_gameID))
            {
                SaveSystem.Save(_gameID, _userData);
            }
        }

        // --- PUBLIC API (Getter / Setter) ---

        public int GetCurrentLevel() => _userData != null ? _userData.CurrentLevelIndex : 0;

        public bool HasProgressInCurrentLevel()
        {
            return _userData != null && _userData.IsLevelInProgress;
        }

        public void MarkLevelStarted()
        {
            if (_userData != null && !_userData.IsLevelInProgress)
            {
                _userData.IsLevelInProgress = true;
                Save();
            }
        }

        public void ClearLevelProgress()
        {
            if (_userData != null)
            {
                _userData.IsLevelInProgress = false;
                Save();
            }
        }

        public void UnlockNextLevel()
        {
            if (_userData != null)
            {
                _userData.CurrentLevelIndex++;
                _userData.IsLevelInProgress = false; 
                Save();
            }
        }

        public void ResetProgress()
        {
            if (_userData != null)
            {
                _userData.CurrentLevelIndex = 0;
                _userData.IsLevelInProgress = false;
                Save();
            }
        }

        public void DeleteSaveFile()
        {
            if (!string.IsNullOrEmpty(_gameID))
            {
                SaveSystem.DeleteFile(_gameID);
                _userData = new WaterSortUserData();
            }
        }
    }
}