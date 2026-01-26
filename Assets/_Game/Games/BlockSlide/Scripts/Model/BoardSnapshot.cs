namespace _Game.Games.BlockSlide.Scripts.Model
{
    public struct BoardSnapshot
    {
        public int [,] BoardData;
        public int Score;

        public BoardSnapshot(int[,] boardData, int score)
        {
            this.BoardData = (int[,])boardData.Clone();
            this.Score = score;
        }
    }
}