using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames
{
    public class BaseDeckWPF<CA, GC, GW> : BaseFrameWPF, IHandle<NewCardEventModel>, IHandleAsync<AnimateCardInPileEventModel<CA>>
            where CA : class, IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsWPF<CA, GC>, new()
    {
        private DeckObservablePile<CA>? _thisMod;
        private GW? _thisObject;
        private SKPoint _objectLocation;
        private Grid? _thisGrid;
        private NumberPieceWPF? _thisNumber;
        private bool _frameRepainted;
        private string _tagUsed = "";
        protected override void RepaintText()
        {
            if (_frameRepainted == false)
            {
                ThisFrame.Text = "";
                ThisDraw.InvalidateVisual();
                _frameRepainted = true;
            }
            if (_thisNumber == null)
                throw new BasicBlankException("It should have created the number text so it can repaint");
            _thisNumber.NumberValue = int.Parse(_thisMod!.TextToAppear);
        }
        public void UpdateDeck(DeckObservablePile<CA> cardModel)
        {
            DataContext = null;
            DataContext = cardModel;
            _thisObject!.DataContext = null;
            _thisObject.DataContext = _thisMod!.CurrentDisplayCard;
        }
        public void Init(DeckObservablePile<CA> cardModel, string tagUsed)
        {
            DataContext = cardModel;
            _tagUsed = tagUsed;
            _thisMod = cardModel;
            if (_thisMod.DrawInCenter == true)
            {
                CanDrawText = false;
                _thisNumber = new NumberPieceWPF();
                _thisNumber.CanHighlight = false; // not this time.
                _thisNumber.TextColor = SKColors.SeaGreen;
                _thisNumber.Margin = new Thickness(2, 2, 2, 2);
            }
            SetBinding(TextProperty, nameof(DeckObservablePile<CA>.TextToAppear));
            SetBinding(IsEnabledProperty, nameof(DeckObservablePile<CA>.IsEnabled));
            _thisGrid = new Grid();
            _thisObject = new GW();
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            CA temps = new CA();
            if (temps.DefaultSize.Height == 0 || temps.DefaultSize.Width == 0)
                throw new BasicBlankException("The width and height cannot be 0.  Rethink");
            _thisObject.SendSize(tagUsed, temps); //i think
            _thisObject.DataContext = cardModel.CurrentDisplayCard;
            Binding thisBind = new Binding(nameof(DeckObservablePile<CA>.DeckObjectClickedCommand));
            thisBind.Source = _thisMod; // has to be that one
            _thisObject.SetBinding(GraphicsCommand.CommandProperty, thisBind);
            //if we need visible, rethink.
            //SetBinding(VisibilityProperty, GetVisibleBinding(nameof(DeckViewModel<CA>.Visible)));
            if (_thisMod.DrawInCenter == false)
            {
                var thisRect = ThisFrame.GetControlArea(); //i think only if no center.  try this way.
                SetUpMarginsOnParentControl(_thisObject, thisRect);
                _objectLocation = new SKPoint(thisRect.Left, thisRect.Top);
            }
            else
            {
                _thisObject.Margin = new Thickness(2, 2, 2, 2);
                _objectLocation = new SKPoint(2, 2);
            }
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this, EnumNewCardCategories.Deck.ToString());
            _thisGrid.Children.Add(ThisDraw);
            _thisGrid.Children.Add(_thisObject);
            if (_thisMod.DrawInCenter == true)
            {
                _thisNumber!.Height = _thisObject.ObjectSize.Height;
                _thisNumber.Width = _thisObject.ObjectSize.Width;
                _thisNumber.NumberValue = int.Parse(_thisMod.TextToAppear);
                _thisNumber.Init();
                _thisGrid.Children.Add(_thisNumber); // so this goes on top  hopefully animations still work (?)
            }
            Content = _thisGrid;
            _thisObject.Init(); // this should be okay.  will do the right thing afterall
        }
        public void Handle(NewCardEventModel message)
        {
            _thisObject!.DataContext = null; //otherwise, does not update properly sometimes.
            _thisObject.DataContext = _thisMod!.CurrentDisplayCard;
        }
        private CustomCanvas? _thisCanvas;
        private AnimateImageClass? _animates;
        private async Task PrepareToAnimateAsync(AnimateCardInPileEventModel<CA> data) // don't need index ever for this.
        {
            var topLocation = GetStartLocation();
            var bottomLocaton = GetBottomLocation();
            switch (data.Direction)
            {
                case EnumAnimcationDirection.StartCardToDown:
                    {
                        _animates!.LocationFrom = _objectLocation;
                        _animates.LocationTo = bottomLocaton;
                        break;
                    }

                case EnumAnimcationDirection.StartCardToUp:
                    {
                        _animates!.LocationFrom = _objectLocation;
                        _animates.LocationTo = topLocation;
                        break;
                    }

                case EnumAnimcationDirection.StartDownToCard:
                    {
                        _animates!.LocationFrom = bottomLocaton;
                        _animates.LocationTo = _objectLocation;
                        break;
                    }

                case EnumAnimcationDirection.StartUpToCard:
                    {
                        _animates!.LocationFrom = topLocation;
                        _animates.LocationTo = _objectLocation;
                        break;
                    }
            }
            GW thisControl = new GW();
            thisControl.SendSize(_tagUsed, data.ThisCard!);
            if (_thisMod!.DeckStyle == DeckObservablePile<CA>.EnumStyleType.Unknown)
                thisControl.IsUnknown = true;
            else
                thisControl.IsUnknown = false;
            thisControl.Drew = false;
            thisControl.IsSelected = false;
            thisControl.DoInvalidate();
            await _animates!.DoAnimateAsync(thisControl); // i think
            thisControl.Visibility = Visibility.Collapsed;
            _thisCanvas!.Children.Clear(); //try this too.
        }
        public void StartListeningMainDeck()
        {
            StartAnimationListener("maindeck");
        }
        private string _animationTag = "";
        public void StartAnimationListener(string animationTag) // has to be manual
        {
            if (_thisCanvas != null)
                throw new BasicBlankException("You already started an animation listener");
            _thisCanvas = new SimpleControls.CustomCanvas();
            _thisGrid!.Children.Add(_thisCanvas);
            _animates = new AnimateImageClass();
            _animates.LongestTravelTime = 200;
            _animates.GameBoard = _thisCanvas;
            IEventAggregator thisE = Resolve<IEventAggregator>();
            thisE.Subscribe(this, animationTag);
            _animationTag = animationTag;
        }
        public void StopListening()
        {
            IEventAggregator thisE = Resolve<IEventAggregator>();
            thisE.Subscribe(this, _animationTag);
            thisE.Unsubscribe(this, "maindeck");
        }
        public async Task HandleAsync(AnimateCardInPileEventModel<CA> message)
        {
            await PrepareToAnimateAsync(message);
        }
    }
}
