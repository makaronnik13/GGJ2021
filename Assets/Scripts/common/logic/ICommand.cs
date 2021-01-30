namespace com.armatur.common.logic
{
    public interface ICommand
    {
        void Process();
        void Undo();
    }
}
