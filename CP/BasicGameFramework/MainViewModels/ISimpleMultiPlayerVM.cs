using BasicGameFramework.ViewModelInterfaces;
namespace BasicGameFramework.MainViewModels
{
    public interface ISimpleMultiPlayerVM : IBlankGameVM
    {
        bool NewRoundVisible { get; set; } //not sure (?)
        bool MainOptionsVisible { get; set; }
        string NormalTurn { get; set; }
        string Status { get; set; }
    }
}