using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXWindowsAndControlsCore.BasicControls.GameFrames
{
    public class BoneYardWPF<CA, GC, GW, LI> : BaseFrameWPF, IHandle<ScatteringCompletedEventModel>
        where CA : ILocationDeck, new()
        where GC : class, IDeckGraphicsCP, new()
        where GW : BaseDeckGraphicsWPF<CA, GC>, new()
        where LI : class, IScatterList<CA>, new()
    {
        private DeckObservableDict<CA>? _objectList;
        private Canvas? _thisCanvas; // no animations are needed here.  even the old did not have it.
        private ScatteringPiecesViewModel<CA, LI>? _thisMod;
        private string _tagUsed = "";
        public void LoadList(ScatteringPiecesViewModel<CA, LI> mod, string tagUsed)
        {
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this);
            SetBinding(TextProperty, nameof(ScatteringPiecesViewModel<CA, LI>.TextToAppear));
            SetBinding(IsEnabledProperty, nameof(ScatteringPiecesViewModel<CA, LI>.IsEnabled));
            _thisMod = mod;
            DataContext = _thisMod; // so it can do the text and isenabled.
            _tagUsed = tagUsed;
            _objectList = mod.RemainingList;
            _objectList.CollectionChanged += ObjectList_CollectionChanged;
            _thisCanvas = new Canvas();
            _thisCanvas.Height = _thisMod.MaxSize.Height;
            _thisCanvas.Width = _thisMod.MaxSize.Width;
            Grid tirstGrid = new Grid();
            tirstGrid.Children.Add(ThisDraw);
            ScrollViewer thisScroll = new ScrollViewer();
            thisScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.Content = _thisCanvas;
            tirstGrid.Children.Add(thisScroll);
            if (Text == "" && _thisMod.TextToAppear != "")
                throw new BasicBlankException("The bindings did not even work for text");
            var thisRect = ThisFrame.GetControlArea();
            thisScroll.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 3, 3, 5); // try this way.
            PopulateCards();
            MouseUp += BoneYardWPF_MouseUp;
            Content = tirstGrid;
        }
        public void UpdateList(ScatteringPiecesViewModel<CA, LI> mod)
        {
            _thisMod = mod;
            DataContext = null;
            DataContext = _thisMod;
            _objectList!.CollectionChanged -= ObjectList_CollectionChanged;
            _objectList = mod.RemainingList;
            _objectList.CollectionChanged += ObjectList_CollectionChanged;
            if (_objectList.Count == 0)
                throw new BasicBlankException("The object list cannot be 0 when updating.  Rethink");
            PopulateCards();
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
            var thisBind = GetCommandBinding(nameof(ScatteringPiecesViewModel<CA, LI>.ObjectCommand));
            thisDeck.SetBinding(BaseDeckGraphicsWPF<CA, GC>.CommandProperty, thisBind);
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
                Canvas.SetTop(thisGraphics, thisCard.Location.Y);
                Canvas.SetLeft(thisGraphics, thisCard.Location.X);
            }
        }
        private GW? FindControl(CA thisCard)
        {
            foreach (var thisCon in _thisCanvas!.Children)
            {
                var deck = (GW)thisCon!;
                if (deck.DataContext.Equals(thisCard) == true)
                    return deck;
            }
            return null;
        }
        private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var thisItem in e.OldItems)
                {
                    var thisCard = (CA)thisItem!;
                    var thisGraphics = FindControl(thisCard);
                    _thisCanvas!.Children.Remove(thisGraphics); // should remove that domino.
                }
            }
        }
        private void BoneYardWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_thisMod!.BoardCommand.CanExecute(null!) == false)
                return;
            _thisMod.BoardCommand.Execute(null!);
        }
        public void Handle(ScatteringCompletedEventModel message)
        {
            PopulateCards();
        }
    }
}