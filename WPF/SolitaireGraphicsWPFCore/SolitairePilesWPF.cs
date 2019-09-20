using BaseSolitaireClassesCP.GraphicsViewModels;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
using System.Windows.Controls;
namespace SolitaireGraphicsWPFCore
{
    public class SolitairePilesWPF : UserControl
    {
        public int Spacing { get; set; } = 0; //defaults to 3 (?)
        private StackPanel? _thisStack;
        private SolitairePilesCP? _thisMod;
        private IndividualSolitairePileWPF? FindControl(PileInfoCP thisPile)
        {
            foreach (var firstControl in _thisStack!.Children)
            {
                var thisGraphics = firstControl as IndividualSolitairePileWPF;
                if (thisGraphics!.DataContext.Equals(thisPile))
                    return thisGraphics;
            }
            return null; //you can have null this time.
        }
        public void ReloadSavedBoard()
        {
            _thisMod!.PileList.ForEach(thisPile =>
            {
                var thisG = FindControl(thisPile);
                thisG!.ThisPile = thisPile;
                thisG.LoadSavedBoard();
            });
        }
        public void Init(SolitairePilesCP thisMod)
        {
            _thisStack = new StackPanel();
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            _thisStack.Orientation = Orientation.Horizontal;
            ScrollViewer thisScroll = new ScrollViewer();
            thisScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _thisMod = thisMod;
            if (thisMod.PileList.Count == 0)
                throw new BasicBlankException("Must have at least one pile.  Otherwise, not worth even using this");
            thisMod.PileList.ForEach(thisPile =>
            {
                IndividualSolitairePileWPF thisG = new IndividualSolitairePileWPF();
                thisG.MainMod = thisMod;
                thisG.ThisPile = thisPile;
                thisG.Margin = new Thickness(0, 0, Spacing, 0);
                _thisStack.Children.Add(thisG);
                thisG.Init();
            });
            thisScroll.Content = _thisStack;
            Content = thisScroll;
        }
    }
}