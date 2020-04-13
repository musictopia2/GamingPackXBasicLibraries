using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIXFLibrary.BasicControls.SingleCardFrames
{
    public class BaseDeckXF<CA, GC, GW> : BaseFrameXF, IHandle<NewCardEventModel>, IHandleAsync<AnimateCardInPileEventModel<CA>>
            where CA : class, IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsXF<CA, GC>, new()
    {
        private DeckObservablePile<CA>? _thisMod;
        private GW? _thisObject;
        private SKPoint _objectLocation;
        private Grid? _thisGrid;
        private NumberPieceXF? _thisNumber;
        private bool _frameRepainted;
        private string _tagUsed = "";
        protected override bool UseLess => true;
        protected override void RepaintText()
        {
            if (_frameRepainted == false)
            {
                ThisFrame.Text = "";
                ThisDraw.InvalidateSurface();
                _frameRepainted = true;
            }
            if (_thisNumber == null)
                throw new BasicBlankException("It should have created the number text so it can repaint");
            _thisNumber.NumberValue = int.Parse(_thisMod!.TextToAppear);
        }

        public void Init(DeckObservablePile<CA> cardModel, string tagUsed) //risk with no property change (?)
        {
            BindingContext = cardModel;
            _tagUsed = tagUsed;
            _thisMod = cardModel;
            if (_thisMod.DrawInCenter == true)
            {
                CanDrawText = false;
                _thisNumber = new NumberPieceXF();
                _thisNumber.CanHighlight = false; // not this time.
                _thisNumber.TextColor = SKColors.SeaGreen;
                _thisNumber.Margin = new Thickness(2, 2, 2, 2);
            }
            SetBinding(TextProperty, new Binding(nameof(DeckObservablePile<CA>.TextToAppear)));
            SetBinding(IsEnabledProperty, new Binding(nameof(DeckObservablePile<CA>.IsEnabled)));
            _thisGrid = new Grid();
            _thisObject = new GW();
            CA temps = new CA();
            if (temps.DefaultSize.Height == 0 || temps.DefaultSize.Width == 0)
                throw new BasicBlankException("The width and height cannot be 0.  Rethink");
            _thisObject.SendSize(tagUsed, temps); //i think
            _thisObject.BindingContext = cardModel.CurrentDisplayCard;
            Binding thisBind = new Binding(nameof(DeckObservablePile<CA>.DeckObjectClickedCommand));
            thisBind.Source = _thisMod; // has to be that one
            _thisObject.SetBinding(BaseDeckGraphicsXF<CA, GC>.CommandProperty, thisBind);
            _thisObject.HorizontalOptions = LayoutOptions.Start;
            _thisObject.VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            if (_thisMod.DrawInCenter == false)
            {
                SetUpMarginsOnParentControl(_thisObject);
                _objectLocation = new SKPoint((float)_thisObject.Margin.Left, (float)_thisObject.Margin.Top);
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
                _thisNumber!.HeightRequest = _thisObject.ObjectSize.Height;
                _thisNumber.WidthRequest = _thisObject.ObjectSize.Width;
                _thisNumber.NumberValue = int.Parse(_thisMod.TextToAppear);
                _thisNumber.Init();
                _thisGrid.Children.Add(_thisNumber); // so this goes on top  hopefully animations still work (?)
            }
            Content = _thisGrid;
            _thisObject.Init(); // this should be okay.  will do the right thing afterall
            ResizeHeight();
            ResizeWidth(); //try this.
        }
        public void Handle(NewCardEventModel message)
        {
            _thisObject!.BindingContext = null; //otherwise, does not update properly sometimes.
            _thisObject.BindingContext = _thisMod!.CurrentDisplayCard;
        }
        private CustomCanvasXF? _thisCanvas;
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
            thisControl.IsVisible = false;
            _thisCanvas!.Children.Clear(); //try this too.
        }
        public void StartListeningMainDeck()
        {
            StartAnimationListener("maindeck");
        }
        private string _animationTag = "";
        public void StartAnimationListener(string animationTag) // has to be manual
        {
            if (_thisCanvas == null == false)
                throw new BasicBlankException("You already started an animation listener");
            _thisCanvas = new CustomCanvasXF();
            _thisCanvas.HeightRequest = 20;
            _thisCanvas.WidthRequest = 20;
            _thisCanvas.InputTransparent = true;
            _thisCanvas.HorizontalOptions = LayoutOptions.Start;
            _thisCanvas.VerticalOptions = LayoutOptions.Start;
            _thisGrid!.Children.Add(_thisCanvas);
            _animates = new AnimateImageClass();
            _animates.LongestTravelTime = 200;
            _animates.GameBoard = _thisCanvas;
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this, animationTag);
            _animationTag = animationTag;
        }
        public async Task HandleAsync(AnimateCardInPileEventModel<CA> message)
        {
            await PrepareToAnimateAsync(message);
        }
        public void ResizeHeight()
        {
            var size = _thisObject!.Measure(double.PositiveInfinity, double.PositiveInfinity);
            HeightRequest = size.Request.Height + Margin.Top + Margin.Bottom + 7;
        }
        public void ResizeWidth()
        {
            var size = _thisObject!.Measure(double.PositiveInfinity, double.PositiveInfinity);
            WidthRequest = size.Request.Width + Margin.Left + Margin.Right;
        }
        public void StopListening()
        {
            IEventAggregator thisE = Resolve<IEventAggregator>();
            thisE.Subscribe(this, _animationTag);
            thisE.Unsubscribe(this, "maindeck");
        }

    }
}