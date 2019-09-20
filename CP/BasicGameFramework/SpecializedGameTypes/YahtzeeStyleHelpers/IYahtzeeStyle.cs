using BasicGameFramework.Dice;
using CommonBasicStandardLibraries.CollectionClasses;
namespace BasicGameFramework.YahtzeeStyleHelpers
{
    /// <summary>
    /// this is all the unique things that has to be done depending on game.
    /// </summary>
    public interface IYahtzeeStyle<D>
        where D : SimpleDice, new()
    {
        CustomBasicList<string> GetBottomText { get; }
        int BonusAmount(int topScore);
        void PopulateBottomScores(ScoresheetVM<D> thisScore);
        CustomBasicList<DiceInformation> GetDiceList(); //i think
        void Extra5OfAKind(ScoresheetVM<D> thisScore); //this is the processes for an extra 5 of a kind.
        bool HasExceptionFor5Kind { get; }//kismet would be true.  others would be false.
    }
}