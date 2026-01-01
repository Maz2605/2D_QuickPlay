using _Game.Core.Scripts.Manager;
using _Game.Games.FruitMerge.Scripts.Model;

namespace _Game.Games.FruitMerge.Scripts.Controller
{
    public class FruitScoreManager : BaseScoreManager<FruitMergeUserData>
    {
        public FruitScoreManager(string gameID) : base(gameID)
        {
        }

        public void AddComboScore(int baseScore, int multiplier)
        {
            int finalScore = baseScore * multiplier;
            
            AddScore(finalScore);

            UserData.totalMerge++;
        }
    }
}