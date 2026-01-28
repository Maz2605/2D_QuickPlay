namespace _Game.Games.WaterSort.Scripts.Model
{
    [System.Serializable]
    public class WaterSortUserData
    {
        public int CurrentLevelIndex { get; set; } = 0; 
        
        public bool IsLevelInProgress = false;
        
        public WaterSortUserData()
        {
            CurrentLevelIndex = 0;
            IsLevelInProgress = false;
        }
    }
}