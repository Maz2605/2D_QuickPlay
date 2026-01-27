using System;
using _Game.Core.Scripts.Data;
using _Game.Core.Scripts.Utils.DesignPattern.Events;

namespace _Game.Core.Scripts.GameSystem
{
    public abstract class EventScoreManager<TUserData, TEventID> : BaseScoreManager<TUserData>
        where TUserData : class, IGameUserData, new()    
        where TEventID : struct, Enum
    {
        private readonly TEventID _scoreEventID;
        private readonly TEventID _highScoreEventID;
        protected EventScoreManager(string gameID, TEventID scoreEventID, TEventID highScoreEventID) : base(gameID)
        {
            _scoreEventID = scoreEventID;
            _highScoreEventID = highScoreEventID;
        }

        public override void AddScore(int amount)
        {
            base.AddScore(amount);
            EventManager<TEventID>.Post(_scoreEventID, CurrentScore);

            if (CurrentScore >= UserData.HighScore)
            {
                EventManager<TEventID>.Post(_highScoreEventID, UserData.HighScore);
            }
        }

        public override void ResetScore()
        {
            base.ResetScore();
            EventManager<TEventID>.Post(_scoreEventID, 0);
        }
        
        public void ForceUpdateEventUI()
        {
            EventManager<TEventID>.Post(_scoreEventID, CurrentScore);
            EventManager<TEventID>.Post(_highScoreEventID, UserData.HighScore);
        }
    }
}