using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIXFLibrary.BasicControls.SingleCardFrames
{
    public class BasePileXF<CA, GC, GW> : BaseFrameXF, IHandle<NewCardEventModel>, IHandleAsync<AnimateCardInPileEventModel<CA>>
        where CA : class, IDeckObject, new()
        where GC : class, IDeckGraphicsCP, new()
        where GW : BaseDeckGraphicsXF<CA, GC>, new()
    {
        private PileObservable<CA>? _thisMod;
        private GW? _thisObject;
        private Grid? _thisGrid;
        private SKPoint _objectLocation;
        private string _tagUsed = "";
        protected override bool UseLess => true;
        public void UpdatePile(PileObservable<CA> cardModel)
        {
            _thisMod = cardModel;
            BindingContext = null;
            BindingContext = _thisMod; //this may be it on this one.
        }
        public void Init(PileObservable<CA> cardModel, string tagUsed)
        {
            BindingContext = cardModel;
            _thisMod = cardModel;
            SetBinding(TextProperty, new Binding(nameof(PileObservable<CA>.Text))); // hopefully the defaults from bindings will appear here.
            SetBinding(IsEnabledProperty, new Binding(nameof(PileObservable<CA>.IsEnabled)));
            _thisGrid = new Grid();
            _thisObject = new GW();
            _tagUsed = tagUsed;
            CA temps = new CA();
            if (temps.DefaultSize.Height == 0 || temps.DefaultSize.Width == 0)
                throw new BasicBlankException("The width and height cannot be 0.  Rethink");
            _thisObject.SendSize(tagUsed, temps); //i think
            _thisObject.BindingContext = cardModel.CurrentDisplayCard;
            _thisObject.HorizontalOptions = LayoutOptions.Start;
            _thisObject.VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            Binding ThisBind = new Binding(nameof(PileObservable<CA>.PileObjectClickedCommand));
            ThisBind.Source = _thisMod; // has to be that one
            _thisObject.SetBinding(BaseDeckGraphicsXF<CA, GC>.CommandProperty, ThisBind);
            SetBinding(IsVisibleProperty, new Binding(nameof(PileObservable<CA>.Visible)));
            SetUpMarginsOnParentControl(_thisObject);
            _objectLocation = new SKPoint((float)_thisObject.Margin.Left, (float)_thisObject.Margin.Top);
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this, EnumNewCardCategories.Discard.ToString());
            _thisGrid.Children.Add(ThisDraw);
            _thisGrid.Children.Add(_thisObject);
            Content = _thisGrid;
            _thisObject.Init(); // this should be okay.  will do the right thing afterall
            ResizeHeight();
            ResizeWidth();
        }
        public void Handle(NewCardEventModel message)
        {
            _thisObject!.BindingContext = null;// not sure why its not updating properly.  trying this way for now.
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
            thisControl.Drew = false; // has to be after the sendsize everytime.
            thisControl.IsSelected = false; // not seleccted either regardless.
            await _animates!.DoAnimateAsync(thisControl); // i think
            if (data.FinalAction != null)
                data.FinalAction.Invoke();
            thisControl.IsVisible = false;
            _thisCanvas!.Children.Clear(); //try this too.
        }
        public async Task HandleAsync(AnimateCardInPileEventModel<CA> message)
        {
            await PrepareToAnimateAsync(message);
        }
        public void StartListeningDiscardPile()
        {
            StartAnimationListener("maindiscard");
        }
        private string _animationTag = "";
        public void StartAnimationListener(string animationTag) // has to be manual
        {
            if (_thisCanvas != null)
                throw new BasicBlankException("You already started an animation listener");
            _thisCanvas = new CustomCanvasXF();
            _thisCanvas.IsEnabled = false; // try this way.
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
        public void StopListening() //this is needed for cases where the main screen may have to be loaded more than once without everything being cleared.  on games like fluxx.
        {
            IEventAggregator thisE = Resolve<IEventAggregator>();
            thisE.Unsubscribe(this, _animationTag);
            thisE.Unsubscribe(this, "maindiscard");
        }
    }
}