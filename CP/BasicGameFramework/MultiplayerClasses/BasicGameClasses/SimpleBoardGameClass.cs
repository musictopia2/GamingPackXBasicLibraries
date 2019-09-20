using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
namespace BasicGameFramework.MultiplayerClasses.BasicGameClasses
{
    public abstract class SimpleBoardGameClass<E, G, P, S, M> : BasicGameClass<P, S>, ISimpleBoardGameProcesses<E, M>
        , IChoosePieceNM, IMoveNM
        where E : struct, Enum
        where P : class, IPlayerBoardGame<E>, new()
        where S : BasicSavedPlainBoardGameClass<E, G, P>, new()
        where G : BaseGraphicsCP, IEnumPiece<E>, new()
    {
        private readonly ISimpleBoardVM<E, G, P> _thisMod;
        public SimpleBoardGameClass(IGamePackageResolver _Container) : base(_Container)
        {
            _thisMod = _Container.Resolve<ISimpleBoardVM<E, G, P>>();
        }
        public bool DidChooseColors { get; set; }
        public override Task StartNewTurnAsync()
        {
            PrepStartTurn();
            
            return Task.CompletedTask;
        }
        protected override void PrepStartTurn()
        {
            base.PrepStartTurn();
            if (DidChooseColors == false)
            {
                _thisMod.ColorChooser!.PlayerList = PlayerList; //should be okay to do it this way
                _thisMod.ColorChooser.Visible = true;
                _thisMod.MainOptionsVisible = false;
                _thisMod.NotifyColorChange();
                _thisMod.ColorChooser.LoadColors();
            }
        }
        public abstract Task MakeMoveAsync(M space);
        public async Task ComputerChooseColorsAsync()
        {
            if (DidChooseColors == true)
                return;
            E thisColor = _thisMod.ColorChooser!.ItemToChoose();
            await ChoseColorAsync(thisColor);
        }
        public override Task ShowWinAsync()
        {
            _thisMod.Instructions = "None";
            return base.ShowWinAsync();
        }
        public override Task ShowTieAsync()
        {
            _thisMod.Instructions = "None";
            return base.ShowTieAsync();
        }
        protected abstract Task AfterChoosingColorsAsync();
        protected virtual void RecordColor() { }//if we ever do risk, will be needed.
        private void CalculateColors()
        {
            DidChooseColors = false;
            foreach (var thisPlayer in PlayerList!)
            {
                if (thisPlayer.DidChooseColor == false && thisPlayer.InGame == true)
                    return;
            }
            DidChooseColors = true;
        }
        public async Task ChoseColorAsync(E thisColor)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("chosepiece", thisColor);
            var tempList = _thisMod.ColorChooser!.ItemList.Select(items => items.EnumValue).ToCustomBasicList();
            tempList.RemoveSpecificItem(thisColor);
            _thisMod.ColorChooser!.ChooseItem(thisColor);
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            SingleInfo!.Color = thisColor;
            RecordColor();
            CalculateColors();
            _thisMod.ColorChooser.Visible = false;
            _thisMod.NotifyColorChange();
            _thisMod.ColorChooser.LoadColors(); //try this
            if (tempList.Count == 1 && DidChooseColors == false)
            {
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                SingleInfo = PlayerList.GetWhoPlayer();
                SingleInfo.Color = tempList.Single();
                CalculateColors();
            }
            if (DidChooseColors == false)
            {
                await EndTurnAsync();
                return;
            }
            _thisMod.MainOptionsVisible = true;
            await AfterChoosingColorsAsync();
        }
        public override bool CanMakeMainOptionsVisibleAtBeginning
        {
            get
            {
                CalculateColors();
                return DidChooseColors;
            }
        }
        /// <summary>
        /// this should only happen upon new game.
        /// </summary>
        protected void EraseColors()
        {
            PlayerList!.ForEach(items =>
            {
                items.Clear();
            });
            DidChooseColors = false;
            SaveRoot!.ThisMod = _thisMod; //i think might as well do here.
            SaveRoot.Instructions = "Choose a color"; //try without prepturn.
        }
        /// <summary>
        /// This will run the processes for the colors parts of it.
        /// </summary>
        protected void BoardGameSaved()
        {
            CalculateColors();
            SaveRoot!.ThisMod = _thisMod;
            _thisMod.Instructions = SaveRoot.Instructions;
            if (CanPrepTurnOnSaved == true)
                PrepStartTurn();
        }
        protected bool CanPrepTurnOnSaved { get; set; } = true;
        async Task IMoveNM.MoveReceivedAsync(string data)
        {
            M item = await js.DeserializeObjectAsync<M>(data);
            await MakeMoveAsync(item);
        }
        async Task IChoosePieceNM.ChoosePieceReceivedAsync(string data)
        {
            E thisColor = await js.DeserializeObjectAsync<E>(data);
            await ChoseColorAsync(thisColor);
        }
    }
}