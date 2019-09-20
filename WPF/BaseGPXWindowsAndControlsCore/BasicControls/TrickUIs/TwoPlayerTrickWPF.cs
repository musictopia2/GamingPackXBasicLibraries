using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.GameGraphicsCP.Animations;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXWindowsAndControlsCore.BasicControls.TrickUIs
{
    public class TwoPlayerTrickWPF<SU, TC, GC, GW> : UserControl, IHandleAsync<AnimateTrickEventModel>
        where SU : struct, Enum
        where TC : class, ITrickCard<SU>, new()
        where GC : class, IDeckGraphicsCP, new()
        where GW : BaseDeckGraphicsWPF<TC, GC>, new()
    {
        private BasicTrickAreaViewModel<SU, TC>? _thisMod;
        private readonly TrickCanvas _thisCanvas;
        private readonly AnimateTrickClass<SU, TC> _animates;
        private DeckObservableDict<TC>? _cardList;
        public void Init(BasicTrickAreaViewModel<SU, TC> thisMod, string tagUsed)
        {
            if (thisMod.CardList.Count != 2)
            {
                throw new BasicBlankException("Only 2 are supported for 2 player trick taking games.  Otherwise, try using another trick taking control");
            }
            DataContext = thisMod;
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
            Canvas.SetLeft(thisLabel, firstLeft);
            Canvas.SetTop(thisLabel, startTop);
            GW thisCard = new GW();
            Binding thisBind = new Binding(nameof(BasicTrickAreaViewModel<SU, TC>.CardSingleClickCommand));
            thisBind.Source = thisMod; // has to be that one
            thisCard.SetBinding(BaseDeckGraphicsWPF<TC, GC>.CommandProperty, thisBind); //i think
            thisCard.SendSize(tagUsed, _cardList.First()); //hopefully this simple.
            thisCard.CommandParameter = _cardList.First(); // this needs to be the parameter.
            _thisCanvas.Children.Add(thisCard);
            Canvas.SetTop(thisCard, cardTop);
            Canvas.SetLeft(thisCard, 10); // its manually done.
            thisLabel = GetLabel("Opponent Card Played");
            _thisCanvas.Children.Add(thisLabel);
            Canvas.SetLeft(thisLabel, 250); // try 150.  can always adjust as needed
            Canvas.SetTop(thisLabel, startTop);
            thisCard = new GW();
            thisCard.SendSize(tagUsed, _cardList.Last()); //hopefully this simple.
            _thisCanvas.Children.Add(thisCard);
            Canvas.SetTop(thisCard, cardTop);
            Canvas.SetLeft(thisCard, 270);
            Binding FinalBind = GetVisibleBinding(nameof(BasicTrickAreaViewModel<SU, TC>.Visible));
            SetBinding(VisibilityProperty, FinalBind);
            Content = _thisCanvas; // this is the content for this page.
        }
        public void Update(BasicTrickAreaViewModel<SU, TC> thisMod)
        {
            _thisMod = thisMod;
            DataContext = thisMod;
            _cardList!.CollectionChanged -= CardList_CollectionChanged;
            _cardList = thisMod.CardList;
            _cardList.CollectionChanged += CardList_CollectionChanged;
            ResetCards(true); //hopefully this simple.
        }
        private TextBlock GetLabel(string text)
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.Foreground = Brushes.Aqua;
            thisLabel.FontWeight = FontWeights.Bold;
            thisLabel.Text = text;
            return thisLabel;
        }
        public TwoPlayerTrickWPF()
        {
            _animates = new AnimateTrickClass<SU, TC>();
            _animates.LongestTravelTime = 100;
            Height = 200;
            _thisCanvas = new TrickCanvas();
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
            thisCard.DataContext = null;
            thisCard.DataContext = _cardList.First();
            thisCard.CommandParameter = _cardList.First();
            if (wasUpdate == false)
                thisCard.IsUnknown = true;
            thisCard = GetCard(1);
            thisCard.DataContext = null;
            thisCard.DataContext = _cardList.Last();
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
                thisCard.DataContext = e.NewItems[0]; // this is the first time i tried with the new function through mvvm helpers.
                thisCard.CommandParameter = e.NewItems[0]!;
                thisCard.RepaintManually(); //maybe this is needed too.  hopefully this simple.  may have to set datacontext to null (?)
                return;
            }
        }
        public async Task HandleAsync(AnimateTrickEventModel message)
        {
            await _animates.MovePiecesAsync(_thisMod!);
        }
    }
}