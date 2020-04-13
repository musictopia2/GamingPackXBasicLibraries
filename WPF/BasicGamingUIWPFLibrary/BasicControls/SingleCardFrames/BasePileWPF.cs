using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.Animations;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames
{
    public class BasePileWPF<CA, GC, GW> : BaseFrameWPF, IHandle<NewCardEventModel>, IHandleAsync<AnimateCardInPileEventModel<CA>>
        where CA : class, IDeckObject, new()
        where GC : class, IDeckGraphicsCP, new()
        where GW : BaseDeckGraphicsWPF<CA, GC>, new()
    {
        private PileObservable<CA>? _thisMod;
        private GW? _thisObject;
        private Grid? _thisGrid;
        private SKPoint _objectLocation;
        private string _tagUsed = "";
        public void UpdatePile(PileObservable<CA> cardModel)
        {
            _thisMod = cardModel;
            DataContext = null;
            DataContext = _thisMod; //this may be it on this one.
        }
        public void Init(PileObservable<CA> cardModel, string tagUsed)
        {
            DataContext = cardModel;
            _thisMod = cardModel;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            SetBinding(TextProperty, nameof(PileObservable<CA>.Text)); // hopefully the defaults from bindings will appear here.
            SetBinding(IsEnabledProperty, nameof(PileObservable<CA>.IsEnabled));
            _thisGrid = new Grid();
            _thisObject = new GW();
            _tagUsed = tagUsed;
            CA temps = new CA();
            if (temps.DefaultSize.Height == 0 || temps.DefaultSize.Width == 0)
                throw new BasicBlankException("The width and height cannot be 0.  Rethink");
            _thisObject.SendSize(tagUsed, temps); //i think
            _thisObject.DataContext = cardModel.CurrentDisplayCard;
            Binding bind = new Binding(nameof(PileObservable<CA>.PileObjectClickedCommand));
            bind.Source = _thisMod; // has to be that one
            _thisObject.SetBinding(GraphicsCommand.CommandProperty, bind);
            
            //if we need visible, rethink.
            SetBinding(VisibilityProperty, GetVisibleBinding(nameof(PileObservable<CA>.Visible)));
            var thisRect = ThisFrame.GetControlArea();
            _thisObject.Margin = new System.Windows.Thickness(thisRect.Left, thisRect.Top, 3, 3); // i think everything else would be fine (well see)
            _objectLocation = new SKPoint(thisRect.Left + 3, thisRect.Top + 3); // i think
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this, EnumNewCardCategories.Discard.ToString());
            _thisGrid.Children.Add(ThisDraw);
            _thisGrid.Children.Add(_thisObject);
            Content = _thisGrid;
            _thisObject.Init(); // this should be okay.  will do the right thing afterall
        }
        public void Handle(NewCardEventModel message)
        {
            _thisObject!.DataContext = null;// not sure why its not updating properly.  trying this way for now.
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
            thisControl.Drew = false; // has to be after the sendsize everytime.
            thisControl.IsSelected = false; // not seleccted either regardless.
            await _animates!.DoAnimateAsync(thisControl); // i think
            if (data.FinalAction != null)
            {
                data.FinalAction.Invoke();
            }
            thisControl.Visibility = Visibility.Collapsed;
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
            _thisCanvas = new CustomCanvas();
            _thisCanvas.IsEnabled = false; // try this way.
            _thisGrid!.Children.Add(_thisCanvas);
            _animationTag = animationTag;
            _animates = new AnimateImageClass();
            _animates.LongestTravelTime = 200;
            _animates.GameBoard = _thisCanvas;
            IEventAggregator thisE = Resolve<IEventAggregator>();
            thisE.Subscribe(this, animationTag);
        }
        public void StopListening() //this is needed for cases where the main screen may have to be loaded more than once without everything being cleared.  on games like fluxx.
        {
            IEventAggregator thisE = Resolve<IEventAggregator>();
            thisE.Unsubscribe(this, _animationTag);
            thisE.Unsubscribe(this, "maindiscard");
        }
    }
}
