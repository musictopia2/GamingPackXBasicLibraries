using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace BasicGameFramework.YahtzeeStyleHelpers
{
    public class YahtzeeViewModel<D> : DiceGamesVM<D, YahtzeePlayerItem<D>, BasicYahtzeeGame<D>>
        where D : SimpleDice, new()
    {
        private int _Round;
        public int Round
        {
            get { return _Round; }
            set
            {
                if (SetProperty(ref _Round, value)) { }
            }
        }
        protected override bool CanRollDice() //for now, this is best.
        {
            return RollNumber <= 2;
        }
        public YahtzeeViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDice()
        {
            return CanRollDice();
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 5;
            ThisCup.ShowHold = true;
            ThisCup.Visible = true;
        }
    }
}