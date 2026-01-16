using System;
using _Game.Core.Scripts.Data;
using UnityEngine;

namespace _Game.Core.Scripts.GameSystem
{
    public class BaseScoreManager<T> where T : class, IGameUserData, new()
    {
        private readonly string _gameID;
        private T _userData;

        public int CurrentScore { get; private set; }
        public int HighScore => _userData.HighScore;

        public event Action<int> OnScoreChanged;
        public event Action<int> OnHighScoreChanged;

        protected BaseScoreManager(string gameID)
        {
            _gameID = gameID;
            LoadData();
            NotifyUI();
        }

        private void LoadData()
        {
            try
            {
                _userData = SaveSystem.Load<T>(_gameID);
            }
            catch (Exception e)
            {
                Debug.LogError($"Lỗi load save: {e.Message}. Tạo data mới.");
            }

            if (_userData == null)
            {
                _userData = new T();
                Debug.LogWarning("UserData bị null, đã tạo mới!");
            }

            CurrentScore = 0;
            NotifyUI();
        }

        public virtual void AddScore(int amount)
        {
            CurrentScore += amount;
            OnScoreChanged?.Invoke(CurrentScore);

            if (CurrentScore > _userData.HighScore)
            {
                _userData.HighScore = CurrentScore;
            }

            NotifyUI();
        }

        public virtual void ResetScore()
        {
            CurrentScore = 0;
        }

        public void Save() => SaveSystem.Save(_gameID, _userData);

        private void NotifyUI()
        {
            OnScoreChanged?.Invoke(CurrentScore);
            OnHighScoreChanged?.Invoke(_userData.HighScore);
        }
    }
}