using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIXFLibrary.BasicControls.TrickUIs
{
    public class TwoPlayerTrickXF<SU, TC, GC, GW> : ContentView, IHandleAsync<AnimateTrickEventModel>
        where SU : struct, Enum
        where TC : class, ITrickCard<SU>, new()
        where GC : class, IDeckGraphicsCP, new()
        where GW : BaseDeckGraphicsXF<TC, GC>, new()
    {
        private BasicTrickAreaObservable<SU, TC>? _thisMod;
        private readonly TrickCanvasXF _thisCanvas;
        private readonly AnimateTrickClass<SU, TC> _animates;
        private DeckObservableDict<TC>? _cardList;
        public void Init(BasicTrickAreaObservable<SU, TC> thisMod, string tagUsed)
        {
            if (thisMod.CardList.Count != 2)
                throw new BasicBlankException("Only 2 are supported for 2 player trick taking games.  Otherwise, try using another trick taking control");
            BindingContext = thisMod;
            _thisMod = thisMod;
            _cardList = thisMod.CardList;
            _cardList.CollectionChanged += CardList_CollectionChanged;
            float startTop;
            float firstLeft;
            startTop = 3;
            firstLeft = 3;
            float cardTop;
            cardTop = 33; // if changing, will be here
            var thisLabel = GetLabel("Your Card Played");
            _thisCanvas.Children.Add(thisLabel);
            var thisRect = new Rectangle(firstLeft, firstLeft, thisLabel.WidthRequest, thisLabel.HeightRequest);
            AbsoluteLayout.SetLayoutBounds(thisLabel, thisRect);
            GW thisCard = new GW();
            Binding thisBind = new Binding(nameof(BasicTrickAreaObservable<SU, TC>.CardSingleClickCommand));
            thisBind.Source = thisMod; // has to be that one
            thisCard.SetBinding(GraphicsCommand.CommandProperty, thisBind); //i think
            thisCard.SendSize(tagUsed, _cardList.First()); //hopefully this simple.
            thisCard.CommandParameter = _cardList.First(); // this needs to be the parameter.
            _thisCanvas.Children.Add(thisCard);
            thisRect = new Rectangle(10, cardTop, thisCard.ObjectSize.Width, thisCard.ObjectSize.Height);
            AbsoluteLayout.SetLayoutBounds(thisCard, thisRect);
            thisLabel = GetLabel("Opponent Card Played");
            _thisCanvas.Children.Add(thisLabel);
            thisRect = new Rectangle(150, startTop, thisLabel.WidthRequest, thisLabel.HeightRequest);
            AbsoluteLayout.SetLayoutBounds(thisLabel, thisRect);
            thisCard = new GW();
            thisCard.SendSize(tagUsed, _cardList.Last()); //hopefully this simple.
            _thisCanvas.Children.Add(thisCard);
            thisRect = new Rectangle(170, cardTop, thisCard.ObjectSize.Width, thisCard.ObjectSize.Height);
            AbsoluteLayout.SetLayoutBounds(thisCard, thisRect);
            Binding FinalBind = new Binding(nameof(BasicTrickAreaObservable<SU, TC>.Visible));
            SetBinding(IsVisibleProperty, FinalBind);
            Content = _thisCanvas; // this is the content for this page.
        }

        private Label GetLabel(string text)
        {
            Label thisLabel = new Label();
            thisLabel.TextColor = Color.Aqua;
            thisLabel.FontAttributes = FontAttributes.Bold;
            thisLabel.Text = text;
            return thisLabel;
        }
        public TwoPlayerTrickXF()
        {
            _animates = new AnimateTrickClass<SU, TC>();
            _animates.LongestTravelTime = 160;
            _thisCanvas = new TrickCanvasXF();
            _animates.GameBoard = _thisCanvas;
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this);
        }
        private GW GetCard(int tndex)
        {
            var ThisControl = _thisCanvas.GetCard(tndex);
            return (GW)ThisControl;
        }
        private void ResetCards(bool wasUpdate)
        {
            var thisCard = GetCard(0);
            thisCard.BindingContext = null;
            thisCard.BindingContext = _cardList.First();
            thisCard.CommandParameter = _cardList.First();
            if (wasUpdate == false)
                thisCard.IsUnknown = true;
            thisCard = GetCard(1);
            thisCard.BindingContext = null;
            thisCard.BindingContext = _cardList.Last();
            if (wasUpdate == false)
                thisCard.IsUnknown = true; // maybe had to do manually
            thisCard.CommandParameter = _cardList.Last();
        }
        private void CardList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ResetCards(false);

                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Count > 1)
                    throw new BasicBlankException("Not sure when there are more than one to replace");
                var index = e.NewStartingIndex; // try new one.  old returned -1 which is not correct.
                var thisCard = GetCard(index);
                thisCard.IsUnknown = false; // needs to be false now.
                thisCard.BindingContext = e.NewItems[0]; // this is the first time i tried with the new function through mvvm helpers.
                thisCard.CommandParameter = e.NewItems[0]!; //hopefully no need to repaint manually (?)
                return;
            }
        }
        public async Task HandleAsync(AnimateTrickEventModel message)
        {
            await _animates.MovePiecesAsync(_thisMod!);
        }
        public void Dispose()
        {
            if (_cardList != null)
            {
                _cardList.CollectionChanged -= CardList_CollectionChanged;
            }
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Unsubscribe(this);
        }
    }
}