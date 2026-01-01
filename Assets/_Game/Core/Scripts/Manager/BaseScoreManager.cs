using System;
using _Game.Core.Scripts.Data;

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
            
        }

        private void LoadData()
        {
            UserData = SaveSystem.Load<T>(GameID);

            if (UserData == null) UserData = new T();

            CurrentScore = 0;
        }

        public virtual void AddScore(int amount)
        {
            CurrentScore += amount;
            OnScoreChanged?.Invoke(CurrentScore);

            if (CurrentScore > UserData.HighScore)
            {
                UserData.HighScore = CurrentScore;
                OnHighScoreChanged?.Invoke(UserData.HighScore);
            }
        }

        public void Save() => SaveSystem.Save(GameID, UserData);

        protected void NotifyUI()
        {
            OnScoreChanged?.Invoke(CurrentScore);
            OnHighScoreChanged?.Invoke(UserData.HighScore);
        }
    }
}