using System.Collections.Generic;
using System.Linq;
using _Game.Core.Scripts.GameSystem;
using _Game.Core.Scripts.Utils.DesignPattern.Events;
using _Game.Games.BlockSlide.Scripts.Config;
using _Game.Games.BlockSlide.Scripts.Model;
using UnityEngine;

namespace _Game.Games.BlockSlide.Scripts.Controller
{
    public class BlockSlideScoreManager : BaseScoreManager<BlockSlideUserData, BlockSlideEventID>
    {
        public BlockSlideScoreManager(string gameID)
            : base(gameID, BlockSlideEventID.ScoreUpdate, BlockSlideEventID.HighScoreUpdate)
        {
            
        }
        
        public void SyncScore(int totalScoreFromModel)
        {
            int delta = totalScoreFromModel - CurrentScore;
            if (delta != 0)
            {
                base.AddScore(delta);
            }
        }

        public void UpdateLeaderboard()
        {
            if (CurrentScore > 0)
            {
                UserData.topScores.Add(CurrentScore);
            }

            UserData.topScores = UserData.topScores
                .OrderByDescending(score => score)
                .Take(3)
                .ToList();
            if (UserData.topScores.Count > 0)
            {
                UserData.HighScore = UserData.topScores[0];
            }

            Save();
        }

        public List<int> GetTopScores()
        {
            return UserData.topScores;
        }
    }
}