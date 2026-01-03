using System;
using _Game.Core.Scripts.Data;
using UnityEngine;

namespace _Game.Core.Scripts.Manager
{
    public class BaseScoreManager<T> where T : class, IGameUserData, new()
    {
        protected string GameID;
        protected T UserData;

        private int CurrentScore { get; set; }
        public int HighScore => UserData.HighScore;

        public event Action<int> OnScoreChanged;
        public event Action<int> OnHighScoreChanged;

        protected BaseScoreManager(string gameID)
        {
            GameID = gameID;
            LoadData();
            NotifyUI();
        }

        private void LoadData()
        {
            try
            {
                UserData = SaveSystem.Load<T>(GameID);
            }
            catch (Exception e)
            {
                Debug.LogError($"Lỗi load save: {e.Message}. Tạo data mới.");
            }

            if (UserData == null)
            {
                UserData = new T();
                Debug.LogWarning("UserData bị null, đã tạo mới!");
            }

            CurrentScore = 0;
            NotifyUI();
        }

        public virtual void AddScore(int amount)
        {
            CurrentScore += amount;
            OnScoreChanged?.Invoke(CurrentScore);

            if (CurrentScore > UserData.HighScore)
            {
                UserData.HighScore = CurrentScore;
            }

            NotifyUI();
        }

        public void Save() => SaveSystem.Save(GameID, UserData);

        protected void NotifyUI()
        {
            OnScoreChanged?.Invoke(CurrentScore);
            OnHighScoreChanged?.Invoke(UserData.HighScore);
        }
    }
}