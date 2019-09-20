namespace BasicGameFramework.CommandClasses
{
    public interface IControlVM
    {
        bool CanExecute();
        void ReportCanExecuteChange();
        EnumCommandBusyCategory BusyCategory { get; set; }
    }
}