using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.MiscProcesses;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.BasicEventModels
{
    public static class EventExtensions
    {
        public static async Task SendLoadAsync(this EventAggregator thisE)
        {
            if (thisE.HandlerExistsFor(typeof(LoadEventModel), Action: EnumActionCategory.Async) == false)
                throw new BasicBlankException("No Event Handler For Loading Async.  Rethink");
            await thisE.PublishAsync(new LoadEventModel());
        }
        public static async Task SendUpdateAsync(this EventAggregator thisE)
        {
            if (thisE.HandlerExistsFor(typeof(UpdateEventModel), Action: EnumActionCategory.Async) == false)
                throw new BasicBlankException("No Event Handler For Update Async.  Rethink");
            await thisE.PublishAsync(new UpdateEventModel());
        }
        public static async Task AnimateMovePiecesAsync<S>(this EventAggregator thisE, Vector previousSpace,
            Vector moveToSpace, S temporaryObject, bool useColumn = false) where S : class
        {
            AnimatePieceEventModel<S> thisA = new AnimatePieceEventModel<S>();
            thisA.MoveToSpace = moveToSpace;
            thisA.PreviousSpace = previousSpace;
            thisA.TemporaryObject = temporaryObject;
            thisA.UseColumn = useColumn;
            await thisE.PublishAsync(thisA);
        }
        public static void RepaintBoard(this EventAggregator thisE)
        {
            thisE.RepaintMessage(EnumRepaintCategories.fromskiasharpboard); //if nothing is specified, then do from skiaboard.
        }
        public static void RepaintMessage(this EventAggregator thisE, EnumRepaintCategories thisCategory)
        {
            thisE.Publish(new RepaintEventModel(), thisCategory.ToString());
        }
        public static void NewCardMessage(this EventAggregator thisE, EnumNewCardCategories thisCategory)
        {
            thisE.Publish(new NewCardEventModel(), thisCategory.ToString());
        }
        #region Animation Objects Helpers
        public static async Task AnimatePlayAsync<D>(this EventAggregator thisE,
            D thisCard, Action? finalAction = null) where D : class, IDeckObject, new()
        {
            await thisE.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartUpToCard, "maindiscard", finalAction: finalAction!);
        }
        public static async Task AnimatePlayAsync<D>(this EventAggregator thisE, D thisCard,
            EnumAnimcationDirection direction, Action? finalAction = null) where D : class, IDeckObject, new()
        {
            await thisE.AnimateCardAsync(thisCard, direction, "maindiscard", finalAction: finalAction!);
        }
        public static async Task AnimateDrawAsync<D>(this EventAggregator thisE, D thisCard) where D : class, IDeckObject, new()
        {
            await thisE.AnimateDrawAsync(thisCard, EnumAnimcationDirection.StartCardToDown);
        }
        public static async Task AnimateDrawAsync<D>(this EventAggregator thisE, D thisCard
            , EnumAnimcationDirection direction) where D : class, IDeckObject, new()
        {
            await thisE.AnimateCardAsync(thisCard, direction, "maindeck");
        }
        public static async Task AnimatePickUpDiscardAsync<D>(this EventAggregator thisE, D thisCard) where D : class, IDeckObject, new()
        {
            await thisE.AnimatePickUpDiscardAsync(thisCard, EnumAnimcationDirection.StartCardToDown);
        }
        public static async Task AnimatePickUpDiscardAsync<D>(this EventAggregator thisE, D thisCard
            , EnumAnimcationDirection direction) where D : class, IDeckObject, new()
        {
            await thisE.AnimateCardAsync(thisCard, direction, "maindiscard");
        }
        public static async Task AnimateCardAsync<D>(this EventAggregator thisE,
            D thisCard, EnumAnimcationDirection direction, string tag
            , BasicPileInfo<D>? pile1 = null, Action? finalAction = null) where D : class, IDeckObject, new()
        {
            AnimateCardInPileEventModel<D> ThisA = new AnimateCardInPileEventModel<D>();
            ThisA.Direction = direction;
            ThisA.FinalAction = finalAction;
            ThisA.ThisCard = thisCard;
            ThisA.ThisPile = pile1;
            await thisE.PublishAsync(ThisA, tag, false);
        }
        #endregion
    }
}