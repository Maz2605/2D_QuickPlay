namespace _Game.Core.Scripts.Utils.DesignPattern.Command
{
    public interface ICommand
    {
        bool Execute();
        void Undo();
    }
}