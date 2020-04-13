using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIXFLibrary.BasicControls.TrickUIs
{
    public class SeveralPlayersTrickXF<SU, TC, GC, GW, PA> : ContentView, IHandleAsync<AnimateTrickEventModel>
            where SU : struct, Enum
            where TC : class, ITrickCard<SU>, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsXF<TC, GC>, new()
            where PA : IPlayerTrick<SU, TC>, new()
    {
        private enum EnumCategory
        {
            Label = 1,
            Card = 2
        }
        private (int Left, int Top) GetCoortinates(EnumCategory category, TrickCoordinate privateCoordinate)
        {
            int newLeft;
            int newTop;
            int x;
            if (privateCoordinate.Row == 1)
            {
                if (category == EnumCategory.Label)
                    newTop = 2;
                else
                    newTop = 27;// i think 15 pixels for label.  if i am wrong, can fix.
                newLeft = 2;
                if (privateCoordinate.Column == 1)
                    return (newLeft, newTop);
                var loopTo = privateCoordinate.Column;
                for (x = 2; x <= loopTo; x++)
                    newLeft += 200;// will do 150 pixels.  that can always change as well.
                return (newLeft, newTop);
            }
            if (privateCoordinate.Row != 2)
                throw new BasicBlankException("Row not supported");
            if (category == EnumCategory.Label)
                newTop = 150;
            else
                newTop = 175;
            newLeft = 2;
            if (privateCoordinate.Column == 1)
                return (newLeft, newTop);
            var loopTo1 = privateCoordinate.Column;
            for (x = 2; x <= loopTo1; x++)
                newLeft += 200;// will do 150 pixels.  that can always change as well.
            return (newLeft, newTop);
        }
        private BasicTrickAreaObservable<SU, TC>? _thisMod;
        private readonly TrickCanvasXF _thisCanvas;
        private readonly AnimateTrickClass<SU, TC> _animates;
        private DeckObservableDict<TC>? _cardList;
        public void Init(BasicTrickAreaObservable<SU, TC> thisMod, IMultiplayerTrick<SU, TC, PA> others, string tagUsed)
        {
            if (thisMod.CardList.Count == 0)
                throw new BasicBlankException("Must have at least one card before i can initialize");
            if (others.ViewList!.Count != thisMod.CardList.Count)
                throw new BasicBlankException("The view list must equal the cardlist as well");
            _thisMod = thisMod;
            _cardList = thisMod.CardList;
            _cardList.CollectionChanged += CardList_CollectionChanged;
            var privateList = others.ViewList.ToCustomBasicList();
            int tempHeight = 0;
            foreach (var thisPrivate in privateList)
            {
                var index = privateList.IndexOf(thisPrivate);
                if (thisPrivate.IsSelf == false && index == 0)
                    throw new BasicBlankException("Must be self to start with");
                var thisPlayer = others.GetSpecificPlayer(thisPrivate.Player); // can send in 1 based
                if (thisPrivate.IsSelf == true && thisPlayer.PlayerCategory != EnumPlayerCategory.Self)
                    throw new BasicBlankException("Self Must Always Produce Self Trick Areas");
                var thisLabel = GetLabel(thisPlayer.NickName, thisPrivate);
                _thisCanvas.Children.Add(thisLabel);
                var thisCoordinate = GetCoortinates(EnumCategory.Label, thisPrivate);
                var thisRect = new Rectangle(thisCoordinate.Left, thisCoordinate.Top, thisLabel.WidthRequest, thisLabel.HeightRequest);
                AbsoluteLayout.SetLayoutBounds(thisLabel, thisRect);
                GW thisCard = new GW();
                Binding thisBind = new Binding(nameof(BasicTrickAreaObservable<SU, TC>.CardSingleClickCommand));
                thisBind.Source = thisMod; // has to be that one
                thisCard.SetBinding(GraphicsCommand.CommandProperty, thisBind); //i think
                var tempCard = thisMod.CardList[index];
                thisCard.SendSize(tagUsed, tempCard);
                thisCard.CommandParameter = tempCard;
                _thisCanvas.Children.Add(thisCard);
                thisCoordinate = GetCoortinates(EnumCategory.Card, thisPrivate);
                thisRect = new Rectangle(thisCoordinate.Left, thisCoordinate.Top, thisCard.ObjectSize.Width, thisCard.ObjectSize.Height);
                AbsoluteLayout.SetLayoutBounds(thisCard, thisRect);
                var OtherHeight = thisCoordinate.Top + thisCard.ObjectSize.Height + 30;
                if (OtherHeight > tempHeight)
                    tempHeight = (int)OtherHeight;
            }
            BindingContext = thisMod;
            Binding finalBind = new Binding(nameof(BasicTrickAreaObservable<SU, TC>.Visible));
            SetBinding(IsVisibleProperty, finalBind);
            Content = _thisCanvas; // this is the content for this page.
        }

        private Label GetLabel(string text, TrickCoordinate thisView)
        {
            Label thisLabel = new Label();
            thisLabel.TextColor = Color.Aqua;
            thisLabel.FontAttributes = FontAttributes.Bold;
            if (!string.IsNullOrEmpty(thisView.Text))
            {
                thisLabel.Text = thisView.Text;
                thisLabel.BindingContext = thisView;
                var ThisBind = new Binding(nameof(TrickCoordinate.Visible));
                thisLabel.SetBinding(IsVisibleProperty, ThisBind); // to hook up so we know if its visible or not.
            }
            else
                thisLabel.Text = text;
            return thisLabel;
        }
        public SeveralPlayersTrickXF()
        {
            _animates = new AnimateTrickClass<SU, TC>();
            _animates.LongestTravelTime = 160;
            _thisCanvas = new TrickCanvasXF();
            _animates.GameBoard = _thisCanvas;
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this);
        }
        private GW GetCard(int index)
        {
            var thisControl = _thisCanvas.GetCard(index);
            return (GW)thisControl;
        }
        private void ResetCards(bool wasUpdate)
        {
            foreach (var tempCard in _thisMod!.CardList)
            {
                int index = _thisMod.CardList.IndexOf(tempCard);
                var thisCard = GetCard(index);
                thisCard.BindingContext = null;
                thisCard.BindingContext = tempCard;
                thisCard.CommandParameter = tempCard;
                if (wasUpdate == false)
                    thisCard.IsUnknown = true;
            }
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
                thisCard.CommandParameter = e.NewItems[0]!; //hopefully does not have to repaint manually now (?)
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