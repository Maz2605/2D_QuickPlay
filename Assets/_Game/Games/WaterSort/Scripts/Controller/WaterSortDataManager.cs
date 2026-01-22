using _Game.Core.Scripts.Data; 
using _Game.Core.Scripts.Utils.DesignPattern.Singleton;
using _Game.Games.WaterSort.Scripts.Config;
using _Game.Games.WaterSort.Scripts.Model; 

namespace _Game.Games.WaterSort.Scripts.Controller
{
    public class WaterSortDataManager : Singleton<WaterSortDataManager>
    {
        private WaterSortUserData _userData;
        private string _gameID;
        private bool _isInitialized = false;

        private WaterSortDataManager() { }

        public void Initialize(string idFromConfig)
        {
            if (_isInitialized) return;
            _gameID = idFromConfig;
            LoadData();
            _isInitialized = true;
        }

        private void LoadData()
        {
            if (string.IsNullOrEmpty(_gameID)) return;
            
            _userData = SaveSystem.Load<WaterSortUserData>(_gameID);
            
            if (_userData == null)
            {
                _userData = new WaterSortUserData();
                _userData.CurrentLevelIndex = 0;
                _userData.IsLevelInProgress = false; 
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
    }
}