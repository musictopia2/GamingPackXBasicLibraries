using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
namespace BasicGamingUIWPFLibrary.BasicControls.TrickUIs
{
    public class SeveralPlayersTrickWPF<SU, TC, GC, GW, PA> : UserControl, IHandleAsync<AnimateTrickEventModel>
            where SU : struct, Enum
            where TC : class, ITrickCard<SU>, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsWPF<TC, GC>, new()
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
                    newLeft += 150;// will do 150 pixels.  that can always change as well.
                return (newLeft, newTop);
            }
            if (privateCoordinate.Row != 2)
                throw new BasicBlankException("Row not supported");
            if (category == EnumCategory.Label)
                newTop = 200;
            else
                newTop = 225;
            newLeft = 2;
            if (privateCoordinate.Column == 1)
                return (newLeft, newTop);
            var loopTo1 = privateCoordinate.Column;
            for (x = 2; x <= loopTo1; x++)
                newLeft += 150;// will do 150 pixels.  that can always change as well.
            return (newLeft, newTop);
        }
        public void Dispose()
        {
            _cardList!.CollectionChanged -= CardList_CollectionChanged;
        }
        private BasicTrickAreaObservable<SU, TC>? _thisMod;
        private readonly TrickCanvas _thisCanvas;
        private readonly AnimateTrickClass<SU, TC> _animates;
        private DeckObservableDict<TC>? _cardList;
        public void Init(BasicTrickAreaObservable<SU, TC> thisMod, IMultiplayerTrick<SU, TC, PA> others, string tagUsed)
        {
            if (thisMod.CardList.Count == 0)
            {
                throw new BasicBlankException("Must have at least one card before i can initialize");
            }
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
                Canvas.SetLeft(thisLabel, thisCoordinate.Left);
                Canvas.SetTop(thisLabel, thisCoordinate.Top);
                GW thisCard = new GW();
                Binding thisBind = new Binding(nameof(BasicTrickAreaObservable<SU, TC>.CardSingleClickCommand));
                thisBind.Source = thisMod; // has to be that one
                thisCard.SetBinding(BaseDeckGraphicsWPF<TC, GC>.CommandProperty, thisBind); //i think
                var tempCard = thisMod.CardList[index];
                thisCard.SendSize(tagUsed, tempCard);
                thisCard.CommandParameter = tempCard;
                _thisCanvas.Children.Add(thisCard);
                thisCoordinate = GetCoortinates(EnumCategory.Card, thisPrivate);
                Canvas.SetLeft(thisCard, thisCoordinate.Left);
                Canvas.SetTop(thisCard, thisCoordinate.Top);
                var OtherHeight = thisCoordinate.Top + thisCard.ObjectSize.Height + 30;
                if (OtherHeight > tempHeight)
                    tempHeight = (int)OtherHeight;
            }
            Height = tempHeight;
            DataContext = thisMod;
            Binding finalBind = GetVisibleBinding(nameof(BasicTrickAreaObservable<SU, TC>.Visible));
            SetBinding(VisibilityProperty, finalBind);
            Content = _thisCanvas; // this is the content for this page.
        }
        
        private TextBlock GetLabel(string text, TrickCoordinate thisView)
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.Foreground = Brushes.Aqua;
            thisLabel.FontWeight = FontWeights.Bold;
            if (!string.IsNullOrEmpty(thisView.Text))
            {
                thisLabel.Text = thisView.Text;
                thisLabel.DataContext = thisView;
                var ThisBind = GetVisibleBinding(nameof(TrickCoordinate.Visible));
                thisLabel.SetBinding(TextBlock.VisibilityProperty, ThisBind); // to hook up so we know if its visible or not.
            }
            else
                thisLabel.Text = text;
            return thisLabel;
        }
        public SeveralPlayersTrickWPF()
        {
            _animates = new AnimateTrickClass<SU, TC>();
            _animates.LongestTravelTime = 160;
            _thisCanvas = new TrickCanvas();
            _animates.GameBoard = _thisCanvas;
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this);
        }
        private GW GetCard(int index)
        {
            var ThisControl = _thisCanvas.GetCard(index);
            return (GW)ThisControl;
        }
        private void ResetCards(bool wasUpdate)
        {
            foreach (var tempCard in _thisMod!.CardList)
            {
                int index = _thisMod.CardList.IndexOf(tempCard);
                var thisCard = GetCard(index);
                thisCard.DataContext = null;
                thisCard.DataContext = tempCard;
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
