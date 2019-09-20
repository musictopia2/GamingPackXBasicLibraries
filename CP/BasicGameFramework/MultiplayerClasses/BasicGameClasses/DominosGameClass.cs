using BasicGameFramework.DIContainers;
using BasicGameFramework.Dominos;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.MultiplayerClasses.BasicGameClasses
{
    public abstract class DominosGameClass<D, P, S> : BasicGameClass<P, S>
        , IDrewDominoNM, IPlayDominoNM, IDominosMainProcesses<D, P>
        where D : IDominoInfo, new()
        where P : class, IPlayerSingleHand<D>, new()
        where S : BasicSavedDominosClass<D, P>, new()
    {
        public int DominosToPassOut { get; set; } //i think this is fine.
        private readonly IDominoGamesVM<D> _thisMod;
        public DominosGameClass(IGamePackageResolver _Container) : base(_Container)
        {
            _thisMod = _Container.Resolve<IDominoGamesVM<D>>();
        }
        protected void LoadUpDominos()
        {
            if (IsLoaded == true)
                throw new BasicBlankException("Should not load the dominos if its already loaded.  Otherwise, rethink");
            _thisMod.PlayerHand1!.Text = "Your Dominos";
            _thisMod.PlayerHand1.Visible = true;
        }
        protected void ClearBoneYard()
        {
            _thisMod.BoneYard!.PopulateBoard();//i think.
        }
        protected void PassDominos()
        {
            if (_thisMod.BoneYard!.RemainingList.Count() == 0)
                throw new BasicBlankException("Cannot have 0 dominos after shuffling");
            PlayerList!.ForEach(ThisPlayer =>
            {
                ThisPlayer.MainHandList.ReplaceRange(_thisMod.BoneYard.FirstDraw(DominosToPassOut));
                if (ThisPlayer.MainHandList.Count() == 0)
                    throw new BasicBlankException("Cannot have 0 dominos when passing out");
            });
            AfterPassedDominos();
        }
        protected void AfterPassedDominos()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.HookUpHand()); //i think this was needed.
            SingleInfo = PlayerList.GetSelf();
            _thisMod.PlayerHand1!.HandList = SingleInfo.MainHandList; //forgot to hook up here.
            SingleInfo.MainHandList.Sort(); //i think.
            _thisMod.BoneYard!.PopulateTotals();
        }
        public async Task DrawDominoAsync(D thisDomino)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
            {
                await ThisNet!.SendAllAsync("drawdomino", thisDomino.Deck);
            }
            _thisMod.BoneYard!.RemoveDomino(thisDomino);
            _thisMod.BoneYard.PopulateTotals();
            thisDomino.IsUnknown = false;
            thisDomino.Drew = true;
            SingleInfo!.MainHandList.Add(thisDomino);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                SingleInfo.MainHandList.Sort();
            await AfterDrawingDomino();
        }
        protected virtual async Task AfterDrawingDomino()
        {
            await ContinueTurnAsync();
        }
        public async Task DrewDominoReceivedAsync(int deck)
        {
            D thisDomino = _thisMod.BoneYard!.RemainingList.GetSpecificItem(deck);
            await DrawDominoAsync(thisDomino);
        }
        protected void ProtectedStartTurn()
        {
            _thisMod.BoneYard!.NewTurn(); //i think
            this.ShowTurn();
        }
        protected void ProtectedSaveBone()
        {
            SaveRoot!.BoneYardData = _thisMod.BoneYard!.SavedData();
        }
        protected void ProtectedLoadBone()
        {
            _thisMod.BoneYard!.SavedGame(SaveRoot!.BoneYardData!);
        }
        protected async Task SendPlayDominoAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == false)
                return;
            await ThisNet!.SendPlayDominoAsync(deck);
        }
        public abstract Task PlayDominoAsync(int Deck); //every game like this requires playing domino.  if i am wrong, rethink
        async Task IPlayDominoNM.PlayDominoMessageAsync(int Deck)
        {
            await PlayDominoAsync(Deck);
        }
    }
}