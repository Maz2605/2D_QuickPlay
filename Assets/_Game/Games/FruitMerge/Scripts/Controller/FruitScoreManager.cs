using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Scripts.GameSystem;
using _Game.Games.FruitMerge.Scripts.Model;
using UnityEngine;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public class FruitScoreManager : BaseScoreManager<FruitMergeUserData>
    {
        private readonly float _comboWindow = 1.0f;
        private float _lastMergeTime;
        private int _currentCombo = 0;
        
        public event Action<int> OnComboUpdated;
        
        public FruitScoreManager(string gameID) : base(gameID)
        {
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
        public override void AddScore(int amount)
        {
            if (Time.time - _lastMergeTime < _comboWindow)
            {
                _currentCombo++;
            }
            else
            {
                _currentCombo = 1; 
            }

            _lastMergeTime = Time.time;
                
            if (_currentCombo > 1) 
            {
                OnComboUpdated?.Invoke(_currentCombo);
            }
            int finalScore = amount * _currentCombo;
            
            base.AddScore(finalScore);
        }

        public override void ResetScore()
        {
            base.ResetScore();
            _currentCombo = 0;
        }
    }
}