namespace _Game.Core.Scripts.Utils.DesignPattern.Command
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}