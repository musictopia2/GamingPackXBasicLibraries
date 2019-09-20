using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.ViewModelInterfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BasicGameFramework.SpecializedGameTypes.RummyClasses
{
    public class RummyHandViewModel<S, C, R> : HandViewModel<R>
        where S : Enum
        where C : Enum
        where R : IDeckObject, IRummmyObject<S, C>, new()
    {
        public event SetClickedEventHandler? SetClickedAsync;
        public delegate Task SetClickedEventHandler(RummyHandViewModel<S, C, R> ThisSet);
        public bool DidClickObject { get; set; } = false; //sometimes this is needed for mobile.
        public void RemoveObject(int deck)
        {
            HandList.RemoveObjectByDeck(deck);
        }
        protected override Task ProcessObjectClickedAsync(R thisObject, int index)
        {
            DidClickObject = true; //this is needed too.  so if other gets raised, will be ignored because already handled.
            thisObject.IsSelected = !thisObject.IsSelected; //try here.  hopefully works well.
            return Task.CompletedTask;
        }
        protected override async Task PrivateBoardSingleClickedAsync()
        {
            if (SetClickedAsync == null)
                return;
            await SetClickedAsync.Invoke(this);
        }
        private ISortObjects<R>? _thisSort;
        public void AddCards(IDeckDict<R> thisList)
        {
            HandList.AddRange(thisList);
            SortCards();
            HandList.UnselectAllObjects();
        }
        private void SortCards()
        {
            if (_thisSort != null)
                HandList.Sort(_thisSort);
            else
                HandList.Sort();
        }
        private void PrepSort(IBasicGameVM thisMod)
        {
            bool rets;
            rets = thisMod.MainContainer!.ObjectExist<ISortObjects<R>>();
            if (rets == true)
            {
                _thisSort = thisMod.MainContainer.Resolve<ISortObjects<R>>();
            }
        }
        public RummyHandViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            PrepSort(thisMod);
        }
    }
}