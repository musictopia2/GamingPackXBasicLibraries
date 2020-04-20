using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFrameworkLibrary.BasicEventModels
{
    public static class EventExtensions
    {

        public static async Task SendGameOverAsync(this IAggregatorContainer aggregator)
        {
            try
            {
                await aggregator.Aggregator.PublishAsync(new GameOverEventModel()); //problem seems to be whoever handles the game over in this case.
            }
            catch (Exception ex)
            {
                UIPlatform.ShowError(ex.Message);
            }
        }
        public static async Task RefreshBindingsAsync(this IUIView view, IEventAggregator aggregator)
        {
            if (aggregator.HandlerExistsFor(typeof(IUIView), action: EnumActionCategory.Async) == false)
            {
                throw new BasicBlankException("No Event Handler For Refresh The Bindings.  Rethink");
            }
            await aggregator.PublishAsync(view);
        }

        public static async Task SendLoadAsync(this IEventAggregator thisE)
        {
            if (thisE.HandlerExistsFor(typeof(LoadEventModel), action: EnumActionCategory.Async) == false)
                throw new BasicBlankException("No Event Handler For Loading Async.  Rethink");
            await thisE.PublishAsync(new LoadEventModel());
        }
        //public static async Task SendUpdateAsync(this IEventAggregator thisE)
        //{
        //    if (thisE.HandlerExistsFor(typeof(UpdateEventModel), action: EnumActionCategory.Async) == false)
        //        throw new BasicBlankException("No Event Handler For Update Async.  Rethink");
        //    await thisE.PublishAsync(new UpdateEventModel());
        //}
        public static async Task AnimateMovePiecesAsync<S>(this IEventAggregator thisE, Vector previousSpace,
            Vector moveToSpace, S temporaryObject, bool useColumn = false) where S : class
        {
            AnimatePieceEventModel<S> thisA = new AnimatePieceEventModel<S>();
            thisA.MoveToSpace = moveToSpace;
            thisA.PreviousSpace = previousSpace;
            thisA.TemporaryObject = temporaryObject;
            thisA.UseColumn = useColumn;
            await thisE.PublishAsync(thisA);
        }
        public static void RepaintBoard(this IEventAggregator thisE)
        {
            thisE.RepaintMessage(EnumRepaintCategories.FromSkiasharpboard); //if nothing is specified, then do from skiaboard.
        }
        public static void RepaintMessage(this IEventAggregator thisE, EnumRepaintCategories thisCategory)
        {
            thisE.Publish(new RepaintEventModel(), thisCategory.ToString());
        }
        public static void NewCardMessage(this IEventAggregator thisE, EnumNewCardCategories thisCategory)
        {
            thisE.Publish(new NewCardEventModel(), thisCategory.ToString());
        }
        #region Animation Objects Helpers
        public static async Task AnimatePlayAsync<D>(this IEventAggregator thisE,
            D thisCard, Action? finalAction = null) where D : class, IDeckObject, new()
        {
            await thisE.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartUpToCard, "maindiscard", finalAction: finalAction!);
        }
        public static async Task AnimatePlayAsync<D>(this IEventAggregator thisE, D thisCard,
            EnumAnimcationDirection direction, Action? finalAction = null) where D : class, IDeckObject, new()
        {
            await thisE.AnimateCardAsync(thisCard, direction, "maindiscard", finalAction: finalAction!);
        }
        public static async Task AnimateDrawAsync<D>(this IEventAggregator thisE, D thisCard) where D : class, IDeckObject, new()
        {
            await thisE.AnimateDrawAsync(thisCard, EnumAnimcationDirection.StartCardToDown);
        }
        public static async Task AnimateDrawAsync<D>(this IEventAggregator thisE, D thisCard
            , EnumAnimcationDirection direction) where D : class, IDeckObject, new()
        {
            await thisE.AnimateCardAsync(thisCard, direction, "maindeck");
        }
        public static async Task AnimatePickUpDiscardAsync<D>(this IEventAggregator thisE, D thisCard) where D : class, IDeckObject, new()
        {
            await thisE.AnimatePickUpDiscardAsync(thisCard, EnumAnimcationDirection.StartCardToDown);
        }
        public static async Task AnimatePickUpDiscardAsync<D>(this IEventAggregator thisE, D thisCard
            , EnumAnimcationDirection direction) where D : class, IDeckObject, new()
        {
            await thisE.AnimateCardAsync(thisCard, direction, "maindiscard");
        }
        public static async Task AnimateCardAsync<D>(this IEventAggregator thisE,
            D thisCard, EnumAnimcationDirection direction, string tag
            , BasicPileInfo<D>? pile1 = null, Action? finalAction = null) where D : class, IDeckObject, new()
        {
            AnimateCardInPileEventModel<D> thisA = new AnimateCardInPileEventModel<D>();
            thisA.Direction = direction;
            thisA.FinalAction = finalAction;
            thisA.ThisCard = thisCard;
            thisA.ThisPile = pile1;
            await thisE.PublishAsync(thisA, tag, false);
        }
        #endregion
    }
}