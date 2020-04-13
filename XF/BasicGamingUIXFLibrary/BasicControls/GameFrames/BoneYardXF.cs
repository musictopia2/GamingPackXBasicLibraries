using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Messenging;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BasicGamingUIXFLibrary.BasicControls.GameFrames
{
    public class BoneYardXF<CA, GC, GW, LI> : BaseFrameXF, IHandle<ScatteringCompletedEventModel>
        where CA : ILocationDeck, new()
        where GC : class, IDeckGraphicsCP, new()
        where GW : BaseDeckGraphicsXF<CA, GC>, new()
        where LI : class, IScatterList<CA>, new()
    {
        private DeckObservableDict<CA>? _objectList;
        private AbsoluteLayout? _thisCanvas; // no animations are needed here.  even the old did not have it.
        private ScatteringPiecesObservable<CA, LI>? _thisMod;
        private string _tagUsed = "";
        public void LoadList(ScatteringPiecesObservable<CA, LI> mod, string tagUsed)
        {
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this);
            SetBinding(TextProperty, new Binding(nameof(ScatteringPiecesObservable<CA, LI>.TextToAppear)));
            SetBinding(IsEnabledProperty, new Binding(nameof(ScatteringPiecesObservable<CA, LI>.IsEnabled)));
            _thisMod = mod;
            BindingContext = _thisMod; // so it can do the text and isenabled.
            _tagUsed = tagUsed;
            _objectList = mod.RemainingList;
            _objectList.CollectionChanged += ObjectList_CollectionChanged;
            _thisCanvas = new AbsoluteLayout();



            _thisCanvas.GestureRecognizers.Add(new TapGestureRecognizer());
            TapGestureRecognizer tap = (TapGestureRecognizer)_thisCanvas.GestureRecognizers.Single();
            tap.NumberOfTapsRequired = 1;
            tap.Command = _thisMod.BoardCommand;
            _thisCanvas.HeightRequest = _thisMod.MaxSize.Height;
            _thisCanvas.WidthRequest = _thisMod.MaxSize.Width;
            Grid firstGrid = new Grid();
            firstGrid.Children.Add(ThisDraw);
            ScrollView thisScroll = new ScrollView();
            thisScroll.Orientation = ScrollOrientation.Both;
            thisScroll.Content = _thisCanvas;
            firstGrid.Children.Add(thisScroll);
            var thisRect = ThisFrame.GetControlArea();
            thisScroll.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 3, 3, 5); // try this way.
            PopulateCards();
            Content = firstGrid;
        }

        private Binding GetCommandBinding(string path)
        {
            Binding thisBind = new Binding(path);
            thisBind.Source = _thisMod;
            return thisBind;
        }
        private void CardBindings(GW thisDeck, CA thisCard)
        {
            thisDeck.SendSize(_tagUsed, thisCard);
            var thisBind = GetCommandBinding(nameof(ScatteringPiecesObservable<CA, LI>.ObjectCommand));
            thisDeck.SetBinding(GraphicsCommand.CommandProperty, thisBind);
            thisDeck.CommandParameter = thisCard;
        }
        private void PopulateCards()
        {
            _thisCanvas!.Children.Clear();
            foreach (var thisCard in _objectList!)
            {
                var thisGraphics = new GW();
                CardBindings(thisGraphics, thisCard);
                _thisCanvas.Children.Add(thisGraphics);
                var bounds = new Rectangle(thisCard.Location.X, thisCard.Location.Y, thisGraphics.ObjectSize.Width, thisGraphics.ObjectSize.Height); //hopefully this works too
                AbsoluteLayout.SetLayoutBounds(thisGraphics, bounds);
            }
        }
        private GW? FindControl(CA thisCard)
        {
            foreach (var thisCon in _thisCanvas!.Children)
            {
                var deck = (GW)thisCon!;
                if (deck.BindingContext.Equals(thisCard) == true)
                    return deck;
            }
            return null;
        }
        private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (var thisItem in e.OldItems)
                {
                    var thisCard = (CA)thisItem!;
                    var thisGraphics = FindControl(thisCard);
                    _thisCanvas!.Children.Remove(thisGraphics); // should remove that domino.
                }
        }
        public void Handle(ScatteringCompletedEventModel message)
        {
            PopulateCards();
        }
    }
}