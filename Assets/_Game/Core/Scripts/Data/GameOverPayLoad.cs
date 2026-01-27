using System.Collections.Generic;

namespace _Game.Core.Scripts.Data
{
    public struct GameOverPayLoad
    {
        public int FinalScore;
        public List<int> TopScores;

        public GameOverPayLoad(int finalScore, List<int> topScores)
        {
            FinalScore = finalScore;
            TopScores = topScores;
        }
    }
}