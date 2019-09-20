using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicControlsAndWindowsCore.Helpers;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using co = BasicControlsAndWindowsCore.BasicWindows.BasicConverters;
namespace BaseGPXWindowsAndControlsCore.BasicControls.GameFrames
{
    public class ScoreBoardWPF : BaseFrameWPF
    {
        private class BindData
        {
            public string MainPath { get; set; }
            public string VisiblePath { get; set; } = "";
            public float RightMargin { get; set; }
            public bool UseTrueFalseConverter { get; set; }
            public bool UseCurrencyConverter { get; set; }
            public BindData(string path)
            {
                MainPath = path;
            }
        }
        private Grid? _mainGrid;
        private CustomBasicList<BindData>? _rowList;
        public bool UseAbbreviationForTrueFalse { get; set; }
        public void AddColumn(string header, bool isHorizontal, string normalPath, string visiblePath = "", float rightMargin = 4, bool useTrueFalse = false, bool useCurrency = false)
        {
            if (useCurrency == true && useTrueFalse == true)
                throw new Exception("You can choose true/false or currency but not both");
            GridHelper.AddAutoColumns(_mainGrid!, 1);
            TextBlock thisLabel = new TextBlock();
            thisLabel.Foreground = Brushes.Aqua;
            thisLabel.FontWeight = FontWeights.Bold;
            BindData thisRow = new BindData(normalPath);
            if (visiblePath != "")
                thisRow.VisiblePath = visiblePath;
            thisRow.UseTrueFalseConverter = useTrueFalse;
            thisRow.UseCurrencyConverter = useCurrency;
            if (rightMargin > 0)
            {
                thisRow.RightMargin = rightMargin;
                thisLabel.VerticalAlignment = VerticalAlignment.Bottom;
            }
            _rowList!.Add(thisRow);
            thisLabel.Text = header;
            if (isHorizontal == false)
            {
                RotateTransform thisRotate = new RotateTransform();
                thisRotate.Angle = 270;
                thisLabel.LayoutTransform = thisRotate;
                thisLabel.Margin = new Thickness(0, 0, 0, 5);
            }
            else
                thisLabel.Margin = new Thickness(0, 0, rightMargin, 5);
            GridHelper.AddControlToGrid(_mainGrid!, thisLabel, 0, _mainGrid!.ColumnDefinitions.Count - 1); // put to last one.
        }
        private CustomBasicList<TextBlock> GetSeveralTextBoxes<P>(P thisPlayer) where P : class, IPlayerItem, new()
        {
            CustomBasicList<TextBlock> thisList = new CustomBasicList<TextBlock>();
            foreach (var firstItem in _mainGrid!.Children)
            {
                TextBlock thisText = (TextBlock)firstItem!;
                if (Grid.GetRow(thisText) > 0)
                {
                    P tempPLayer = (P)thisText.DataContext;
                    if (tempPLayer.Id == thisPlayer.Id)
                        thisList.Add(thisText);
                }
            }
            if (thisList.Count == 0)
                throw new BasicBlankException($"Could not find any textboxes for player {thisPlayer.NickName}");
            if (thisList.Count != _rowList!.Count)
                throw new BasicBlankException($"Textboxes don't reconcile.  Found {thisList.Count} but needed {_rowList.Count}");
            return thisList;
        }
        public void UpdateLists<P>(PlayerCollection<P> thisList) where P : class, IPlayerItem, new()
        {
            foreach (var thisItem in thisList)
            {
                CustomBasicList<TextBlock> newList = GetSeveralTextBoxes(thisItem);
                foreach (var thisLabel in newList)
                {
                    thisLabel.DataContext = null; //try that first.
                    thisLabel.DataContext = thisItem;
                }
            }
        }
        public void LoadLists<P>(PlayerCollection<P> thisList) where P : class, IPlayerItem, new()
        {
            int x = 0;
            int y;
            co.VisibilityConverter thisV = new co.VisibilityConverter();
            GridHelper.AddAutoRows(_mainGrid!, thisList.Count());
            co.TrueFalseConverter thisT = null!;
            co.CurrencyConverter thisC = null!;
            if (_rowList.Any(Items => Items.UseTrueFalseConverter == true))
            {
                thisT = new co.TrueFalseConverter();
                thisT.UseAbb = UseAbbreviationForTrueFalse;
            }
            if (_rowList.Any(Items => Items.UseCurrencyConverter == true))
                thisC = new co.CurrencyConverter();
            foreach (var thisItem in thisList)
            {
                x += 1;
                y = 0;
                foreach (var thisBind in _rowList!)
                {
                    TextBlock thisLabel = new TextBlock();
                    thisLabel.DataContext = thisItem;
                    thisLabel.Foreground = Brushes.Aqua;
                    if (thisBind.UseTrueFalseConverter == true)
                    {
                        Binding tss = new Binding(thisBind.MainPath);
                        tss.Converter = thisT;
                        thisLabel.SetBinding(TextBlock.TextProperty, tss);
                    }
                    else if (thisBind.UseCurrencyConverter == true)
                    {
                        Binding css = new Binding(thisBind.MainPath);
                        css.Converter = thisC;
                        thisLabel.SetBinding(TextBlock.TextProperty, css);
                    }
                    else
                        thisLabel.SetBinding(TextBlock.TextProperty, new Binding(thisBind.MainPath));
                    if (thisBind.VisiblePath != "")
                    {
                        Binding otherBind = new Binding(thisBind.VisiblePath);
                        otherBind.Converter = thisV;
                        thisLabel.SetBinding(TextBlock.VisibilityProperty, otherBind);
                    }
                    if (thisBind.RightMargin > 0)
                        thisLabel.Margin = new Thickness(0, 0, thisBind.RightMargin, 0);
                    else
                        thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    GridHelper.AddControlToGrid(_mainGrid!, thisLabel, x, y);
                    y += 1;
                }
            }
        }
        protected override void SetPaintDimensions(float width, float height)
        {
            ThisFrame.ActualHeight = height - 2;
        }
        protected override void FirstSetUp()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top; // sometimes has to manually do though.
            base.FirstSetUp();
            Grid firstGrid = new Grid();
            firstGrid.Children.Add(ThisDraw);
            ScrollViewer thisScroll = new ScrollViewer();
            Text = "Score Details";
            var thisRect = ThisFrame.GetControlArea();
            thisScroll.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 3, 3, 5); // try this way.
            _mainGrid = new Grid();
            thisScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.Content = _mainGrid;
            firstGrid.Children.Add(thisScroll);
            Content = firstGrid;
            GridHelper.AddAutoRows(_mainGrid, 1);
            _rowList = new CustomBasicList<BindData>();
            AddColumn("Nick Name", true, "NickName", rightMargin: 10);
        }
    }
}