using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers;
using System;
using System.Linq; //sometimes i do use linq.
using System.Threading.Tasks;
namespace BasicGameFramework.SpecializedGameTypes.RummyClasses
{
    public class TempSetsViewModel<S, C, R> : ObservableObject
        where S : Enum
        where C : Enum
        where R : IDeckObject, IRummmyObject<S, C>, new()
    {
        public event SetClickedEventHandler? SetClickedAsync;
        public delegate Task SetClickedEventHandler(int Index);
        public int Spacing { get; set; }
        public int HowManySets { get; set; } = 5; // defaults at 5
        public CustomBasicList<RummyHandViewModel<S, C, R>> SetList = new CustomBasicList<RummyHandViewModel<S, C, R>>(); //has to be public because of data binding
        public DeckObservableDict<R> ObjectList(int index)
        {
            return SetList[index - 1].HandList; //i send in one based.
        }
        private EventAggregator? _thisE;
        public void Init(IBasicGameVM thisMod)
        {
            _thisE = thisMod.MainContainer!.Resolve<EventAggregator>();
            IEnableAlways fins = thisMod.MainContainer.Resolve<IEnableAlways>();
            int x;
            var loopTo = HowManySets;
            RummyHandViewModel<S, C, R> thisSet;
            for (x = 1; x <= loopTo; x++)
            {
                thisSet = new RummyHandViewModel<S, C, R>(thisMod);
                thisSet.AutoSelect = DrawableListsViewModels.HandViewModel<R>.EnumAutoType.None; // somehow doing selectasmany is not working.  this means if its select all, then will be done on the event.
                thisSet.Visible = true;
                thisSet.SendAlwaysEnable(fins); //i think.
                thisSet.Text = "Set"; // will always be set on this one.
                thisSet.SetClickedAsync += ThisSet_SetClickedAsync;
                SetList.Add(thisSet);
            }
        }
        private async Task ThisSet_SetClickedAsync(RummyHandViewModel<S, C, R> thisSet)
        {
            if (SetClickedAsync == null)
                return;
            if (thisSet.DidClickObject == true)
            {
                thisSet.DidClickObject = false;
                return;
            }
            await SetClickedAsync.Invoke(SetList.IndexOf(thisSet) + 1); //wanted to make it one based.
        }
        public void ResetCards()
        {
            SetList.ForEach(Items => Items.DidClickObject = false);
        }
        public DeckRegularDict<R> ListAllObjects()
        {
            DeckRegularDict<R> output = new DeckRegularDict<R>();
            SetList.ForEach(thisTemp =>
            {
                output.AddRange(thisTemp.HandList);
            });
            return output;
        }
        public void UnselectAllCards()
        {
            SetList.ForEach(items => items.UnselectAllObjects());
        }
        public void EndTurn()
        {
            SetList.ForEach(items => items.EndTurn());
        }
        public void ClearBoard()
        {
            SetList.ForEach(items => items.ClearHand());
            PublicCount();
        }
        public void ClearBoard(int index)
        {
            SetList[index - 1].ClearHand(); //sending in one based.
            PublicCount();
        }
        public void PublicCount()
        {
            UpdateCountEventModel thisU = new UpdateCountEventModel();
            thisU.ObjectCount = TotalObjects;
            _thisE!.Publish(thisU);
        }
        public int TotalObjects => SetList.Sum(Items => Items.HandList.Count);
        public int HowManySelectedObjects => SetList.Sum(Items => Items.HowManySelectedObjects);
        public bool HasSelectedObject => SetList.Any(Items => Items.HowManySelectedObjects > 0);
        public int PileForSelectedObject
        {
            get
            {
                RummyHandViewModel<S, C, R> thisVM = SetList.FirstOrDefault(Items => Items.HowManySelectedObjects > 0);
                if (thisVM == null)
                    throw new BasicBlankException("There was no pile with only one selected card.  Find out what happened");
                return SetList.IndexOf(thisVM) + 1; //returning 1 based.
            }
        }
        public int DeckForSelectedObjected(int pile)
        {
            return SetList[pile - 1].ObjectSelected();
        }
        public DeckRegularDict<R> ListObjectsRemoved()
        {
            var output = ListAllObjects();
            ClearBoard();
            return output;
        }
        public bool HasObject(int deck)
        {
            var thisList = ListAllObjects();
            return thisList.ObjectExist(deck);
        }
        public void RemoveObject(int deck)
        {
            foreach (var thisSet in SetList)
            {
                if (thisSet.HandList.ObjectExist(deck))
                {
                    thisSet.HandList.RemoveObjectByDeck(deck);
                    PublicCount(); //because it changed now.
                    return;
                }
            }
            throw new BasicBlankException($"There is no card with the deck {deck} to remove for tempsets");
        }
        public DeckRegularDict<R> ListSelectedObjects(bool alsoRemove = false)
        {
            DeckRegularDict<R> output = new DeckRegularDict<R>();
            SetList.ForEach(thisTemp =>
            {
                output.AddRange(thisTemp.ListSelectedObjects(alsoRemove));
            });
            if (alsoRemove == true)
                PublicCount();
            return output;
        }
        public DeckRegularDict<R> SelectedObjectsRemoved()
        {
            return ListSelectedObjects(true);
        }
        public R GetSelectedObject => ListSelectedObjects().Single();
        public void AddCards(int whichOne, IDeckDict<R> cardList)
        {
            var tempList = SelectedObjectsRemoved();
            cardList.AddRange(tempList);
            if (cardList.Count == 0)
                return;
            SetList[whichOne - 1].AddCards(cardList);
            PublicCount();
        }
    }
}