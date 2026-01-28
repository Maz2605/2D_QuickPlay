using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Scripts.GameSystem;
using _Game.Games.FruitMerge.Scripts.Config;
using _Game.Games.FruitMerge.Scripts.Model;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public class FruitScoreManager : BaseScoreManager<FruitMergeUserData, FruitMergeEventID>
    {
        private float _lastMergeTime;
        private int _currentCombo = 1;
        private const float COMBO_TIMEOUT = 1.5f;

        public FruitScoreManager(string gameID) 
            : base(gameID, FruitMergeEventID.ScoreUpdated, FruitMergeEventID.HighScoreUpdated)
        {
            
        }

        public void HandleMergeScore(int baseScore)
        {
            if (Time.time - _lastMergeTime < COMBO_TIMEOUT)
            {
                _currentCombo++;
            }
            else
            {
                _currentCombo = 1;
            }
            _lastMergeTime = Time.time;

            int finalScore = baseScore * _currentCombo;

            AddScore(finalScore);
        }

        public void UpdateLeaderBoard()
        {
            if (CurrentScore > 0)
            {
                UserData.topScores.Add(CurrentScore);
            }

            UserData.topScores = UserData.topScores.OrderByDescending(x => x).Take(3).ToList();

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