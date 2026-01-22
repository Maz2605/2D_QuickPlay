namespace _Game.Games.WaterSort.Scripts.Config
{
    [System.Serializable]
    public class WaterSortUserData
    {
        public int CurrentLevelIndex { get; set; } = 0; 
        
        public bool IsLevelInProgress = false;
    }
}