using BasicGameFramework.ViewModelInterfaces;
namespace BaseSolitaireClassesCP.BasicVMInterfaces
{
    public interface IBasicScoreVM : IBasicGameVM
    {
        int Score { get; set; } //needs to have something that does score.  will be other basic interfaces to implement.
    }
}