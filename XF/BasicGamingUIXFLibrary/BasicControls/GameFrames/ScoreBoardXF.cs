using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using Xamarin.Forms;
using co = BasicXFControlsAndPages.Converters;
namespace BasicGamingUIXFLibrary.BasicControls.GameFrames
{
    public class ScoreBoardXF : BaseFrameXF
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
            BindData thisRow = new BindData(normalPath);
            if (visiblePath != "")
                thisRow.VisiblePath = visiblePath;
            thisRow.UseTrueFalseConverter = useTrueFalse;
            thisRow.UseCurrencyConverter = useCurrency;
            if (rightMargin > 0)
                thisRow.RightMargin = rightMargin;
            _rowList!.Add(thisRow);
            if (isHorizontal)
            {
                Label firstLabel = new Label();
                firstLabel.TextColor = Color.Aqua;
                firstLabel.FontAttributes = FontAttributes.Bold;
                firstLabel.Text = header;
                firstLabel.Margin = new Thickness(0, 0, 0, 5);
                firstLabel.VerticalOptions = LayoutOptions.End;
                GridHelper.AddControlToGrid(_mainGrid!, firstLabel, 0, _mainGrid!.ColumnDefinitions.Count - 1);
            }
            else
            {
                RotatedLabelXF roLabel = new RotatedLabelXF(header);
                GridHelper.AddControlToGrid(_mainGrid!, roLabel, 0, _mainGrid!.ColumnDefinitions.Count - 1);
            }
        }
        private CustomBasicList<Label> GetSeveralTextBoxes<P>(P thisPlayer) where P : class, IPlayerItem, new()
        {
            CustomBasicList<Label> thisList = new CustomBasicList<Label>();
            foreach (var firstItem in _mainGrid!.Children)
                if (Grid.GetRow(firstItem) > 0)
                {
                    P tempPLayer = (P)firstItem.BindingContext;
                    if (tempPLayer.Id == thisPlayer.Id)
                        thisList.Add((Label)firstItem);
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
                CustomBasicList<Label> newList = GetSeveralTextBoxes(thisItem);
                foreach (var thisLabel in newList)
                {
                    thisLabel.BindingContext = null; //try that first.
                    thisLabel.BindingContext = thisItem;
                }
            }
        }
        public void LoadLists<P>(PlayerCollection<P> thisList) where P : class, IPlayerItem, new()
        {
            int x = 0;
            int y;
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
                    Label thisLabel = new Label();
                    thisLabel.BindingContext = thisItem;
                    thisLabel.TextColor = Color.Aqua;
                    if (thisBind.UseTrueFalseConverter == true)
                    {
                        Binding tss = new Binding(thisBind.MainPath);
                        tss.Converter = thisT;
                        thisLabel.SetBinding(Label.TextProperty, tss);
                    }
                    else if (thisBind.UseCurrencyConverter == true)
                    {
                        Binding css = new Binding(thisBind.MainPath);
                        css.Converter = thisC;
                        thisLabel.SetBinding(Label.TextProperty, css);
                    }
                    else
                        thisLabel.SetBinding(Label.TextProperty, new Binding(thisBind.MainPath));
                    if (thisBind.VisiblePath != "")
                    {
                        Binding otherBind = new Binding(thisBind.VisiblePath);
                        thisLabel.SetBinding(IsVisibleProperty, otherBind);
                    }
                    if (thisBind.RightMargin > 0)
                        thisLabel.Margin = new Thickness(0, 0, thisBind.RightMargin, 0);
                    else
                    {
                        thisLabel.HorizontalOptions = LayoutOptions.Start;
                        thisLabel.HorizontalTextAlignment = TextAlignment.Start;
                    }
                    GridHelper.AddControlToGrid(_mainGrid!, thisLabel, x, y);
                    y += 1;
                }
            }
        }
        protected override void FirstSetUp()
        {
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            Grid firstGrid = new Grid();
            firstGrid.Children.Add(ThisDraw);
            ScrollView thisScroll = new ScrollView();
            Text = "Score Details";
            var thisRect = ThisFrame.GetControlArea();
            thisScroll.Margin = new Thickness(thisRect.Left, thisRect.Top, 3, 2);
            _mainGrid = new Grid();
            _mainGrid.RowSpacing = 0;
            _mainGrid.ColumnSpacing = 4;
            thisScroll.Orientation = ScrollOrientation.Both;
            thisScroll.Content = _mainGrid;
            firstGrid.Children.Add(thisScroll);
            Content = firstGrid;
            GridHelper.AddAutoRows(_mainGrid, 1);
            _rowList = new CustomBasicList<BindData>();
            AddColumn("Nick Name", true, "NickName", rightMargin: 8);
            firstGrid.HorizontalOptions = LayoutOptions.Start;
            firstGrid.VerticalOptions = LayoutOptions.Start;
        }
    }
}