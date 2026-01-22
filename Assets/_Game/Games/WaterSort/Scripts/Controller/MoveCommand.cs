namespace _Game.Games.WaterSort.Scripts.Controller
{
    public class MoveCommand
    {
        public int FromBottleIndex;
        public int ToBottleIndex;
        public int Amount;
        public int ColorId;

        public MoveCommand(int from, int to, int amount, int color)
        {
            FromBottleIndex = from;
            ToBottleIndex = to;
            Amount = amount;
            ColorId = color;
        }
    }
}