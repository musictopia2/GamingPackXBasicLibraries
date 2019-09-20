using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.Dice;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFramework.YahtzeeStyleHelpers;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXPagesAndControlsXF.BasicControls.YahtzeeControls
{
    public class ScoresheetXF<D> : ContentView
        where D : SimpleDice, new()
    {
        private IYahtzeeLayout? _layout;
        private IStandardScreen? _screen;
        public void Init(ScoresheetVM<D> thisMod)
        {
            _layout = Resolve<IYahtzeeLayout>(); //can't do unit testing here anyways.
            _screen = Resolve<IStandardScreen>();
            Binding thisBind = new Binding(nameof(ScoresheetVM<D>.Visible));
            BindingContext = thisMod;
            SetBinding(IsVisibleProperty, thisBind);
            Grid thisGrid = new Grid();
            2.Times(x => GridHelper.AddLeftOverColumn(thisGrid, 50));
            thisGrid.RowSpacing = 2;
            CrossPlatformBorderXF thisBorder; //no groupboxes.
            Grid otherGrid = new Grid();
            otherGrid.BackgroundColor = Color.White;
            if (_screen.IsSmallest)
            {
                otherGrid.HorizontalOptions = LayoutOptions.Start;
                otherGrid.VerticalOptions = LayoutOptions.Start;
            }
            otherGrid.Margin = new Thickness(4, 4, 4, 4);
            otherGrid.RowSpacing = 0;
            otherGrid.ColumnSpacing = 0;
            GridHelper.AddControlToGrid(thisGrid, otherGrid, 0, 0); //i think
            GridHelper.AddLeftOverColumn(otherGrid, 20);
            GridHelper.AddLeftOverColumn(otherGrid, 15);
            GridHelper.AddLeftOverColumn(otherGrid, 15);
            int pixelHeight = _layout.GetPixelHeight;
            8.Times(y => GridHelper.AddPixelRow(otherGrid, pixelHeight));
            Label thisLabel;
            var thisList = (from Items in thisMod.RowList
                            where Items.IsTop == true && Items.RowSection == EnumRowEnum.Regular
                            select Items).ToCustomBasicList();
            if (thisList.Count != 6)
                throw new BasicBlankException("All yahtzee games must have 6 sections total, not " + thisList.Count);
            int x = 0;
            foreach (var thisRow in thisList)
            {
                x += 1;
                AddBackroundLabel(otherGrid, thisRow, x);
                thisLabel = GetDescriptionText(thisRow.Description);
                thisBorder = GetBorder();
                GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 0);
                GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 0);
                thisLabel = GetPossibleText(thisRow);
                GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 1);
                thisBorder = GetBorder();
                GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 1);
                thisLabel = GetScoreText(thisRow);
                GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 2);
                thisBorder = GetBorder();
                GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 2);
                RowClickerXF TempRow = new RowClickerXF();
                GridHelper.AddControlToGrid(otherGrid, TempRow, x, 0);
                TempRow.BindingContext = thisRow; //i think.
                TempRow.Command = thisMod.RowClickedCommand; //i think.
                Grid.SetColumnSpan(TempRow, 3);
            }
            RowInfo bonus = (from Items in thisMod.RowList
                             where Items.IsTop == true && Items.RowSection == EnumRowEnum.Bonus
                             select Items).Single();
            x += 1;
            thisLabel = GetDescriptionText(bonus.Description);
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 0);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 0);
            thisLabel = new Label();
            thisLabel.BackgroundColor = Color.Aqua;
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 1);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 1);
            thisLabel = GetScoreText(bonus);
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 2);
            x += 1; //total score last
            thisLabel = GetFooterText("Upper Total");
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 0);
            Grid.SetColumnSpan(thisLabel, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 0);
            Grid.SetColumnSpan(thisBorder, 2);
            RowInfo topTotal = (from Items in thisMod.RowList
                                where Items.IsTop == true && Items.RowSection == EnumRowEnum.Totals
                                select Items).Single();
            thisLabel = GetScoreText(topTotal);
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 2);
            otherGrid = new Grid();
            GridHelper.AddControlToGrid(thisGrid, otherGrid, 0, 1); //i think
            otherGrid.BackgroundColor = Color.White;
            otherGrid.RowSpacing = 0;
            otherGrid.ColumnSpacing = 0;
            otherGrid.Margin = new Thickness(4, 4, 4, 4);
            if (_screen.IsSmallest)
            {
                otherGrid.HorizontalOptions = LayoutOptions.Start;
                otherGrid.VerticalOptions = LayoutOptions.Start;
            }
            GridHelper.AddLeftOverColumn(otherGrid, 20);
            GridHelper.AddLeftOverColumn(otherGrid, 15);
            GridHelper.AddLeftOverColumn(otherGrid, 15);
            thisList = (from Items in thisMod.RowList
                        where Items.IsTop == false && Items.RowSection == EnumRowEnum.Regular
                        select Items).ToCustomBasicList();
            int maxs = thisList.Count + 1;
            maxs.Times(x => GridHelper.AddPixelRow(otherGrid, pixelHeight));
            x = 0;
            foreach (var thisRow in thisList)
            {
                RowClickerXF tempRow = new RowClickerXF();
                AddBackroundLabel(otherGrid, thisRow, x);
                thisLabel = GetDescriptionText(thisRow.Description);
                thisBorder = GetBorder();
                GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 0);
                GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 0);
                thisLabel = GetPossibleText(thisRow);
                GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 1);
                thisBorder = GetBorder();
                GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 1);
                thisLabel = GetScoreText(thisRow);
                GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 2);
                thisBorder = GetBorder();
                GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 2);
                GridHelper.AddControlToGrid(otherGrid, tempRow, x, 0);
                tempRow.Command = thisMod.RowClickedCommand;
                tempRow.BindingContext = thisRow;
                Grid.SetColumnSpan(tempRow, 3);
                x += 1; //risking here.
            }
            RowInfo bottomTotal = (from Items in thisMod.RowList
                                   where Items.IsTop == false && Items.RowSection == EnumRowEnum.Totals
                                   select Items).Single();
            thisLabel = GetFooterText("Total Bottom Portion");
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 0);
            Grid.SetColumnSpan(thisLabel, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 0);
            Grid.SetColumnSpan(thisBorder, 2);
            thisLabel = GetScoreText(bottomTotal);
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 2);
            Content = thisGrid;
        }
        private void AddBackroundLabel(Grid otherGrid, RowInfo thisRow, int row)
        {
            Label thisText = new Label();
            {
                var withBlock = thisText;
                withBlock.BindingContext = thisRow;
                ColorConverter ThisCon = new ColorConverter();
                Binding ThisBind = new Binding(nameof(thisRow.IsRecent));
                ThisBind.Converter = ThisCon;
                withBlock.SetBinding(BackgroundColorProperty, ThisBind);
                GridHelper.AddControlToGrid(otherGrid, thisText, row, 0);
                Grid.SetColumnSpan(thisText, 3);
            }
        }
        private Label GetFooterText(string text)
        {
            Label thisLabel = new Label();
            thisLabel.FontSize = _layout!.FooterFontSize;
            thisLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            thisLabel.TextColor = Color.Black;
            thisLabel.VerticalOptions = LayoutOptions.Center;
            thisLabel.Text = text;
            return thisLabel;
        }
        private CrossPlatformBorderXF GetBorder()
        {
            CrossPlatformBorderXF thisBorder = new CrossPlatformBorderXF();
            return thisBorder;
        }
        private Label GetPossibleText(RowInfo thisRow)
        {
            Label thisLabel = new Label();
            thisLabel.FontSize = _layout!.StandardFontSize; // 30 did not work for making larger text.
            thisLabel.TextColor = Color.Red;
            thisLabel.BindingContext = thisRow;
            thisLabel.SetBinding(Label.TextProperty, new Binding(nameof(thisRow.Possible)));
            SetCenterLabel(thisLabel);
            return thisLabel;
        }
        private void SetCenterLabel(Label thisLabel)
        {
            if (_screen!.IsSmallest)
            {
                thisLabel.HorizontalOptions = LayoutOptions.Center;
                thisLabel.VerticalOptions = LayoutOptions.Center;
            }
            else
            {
                thisLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
                thisLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
            }
        }
        private Label GetScoreText(RowInfo thisRow)
        {
            Label thisLabel = new Label();
            SetCenterLabel(thisLabel);
            thisLabel.FontSize = _layout!.StandardFontSize;
            thisLabel.TextColor = Color.Black;
            thisLabel.BindingContext = thisRow;
            thisLabel.SetBinding(Label.TextProperty, new Binding(nameof(thisRow.PointsObtained)));
            if (_screen!.IsSmallest)
            {
                thisLabel.HorizontalOptions = LayoutOptions.Center;
                thisLabel.VerticalOptions = LayoutOptions.Center;
            }
            else
            {
                thisLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
                thisLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
            }
            return thisLabel;
        }
        private Label GetDescriptionText(string text)
        {
            Label thisLabel = new Label();
            thisLabel.FontSize = _layout!.DescriptionFontSize;
            SetCenterLabel(thisLabel);
            thisLabel.TextColor = Color.Black;
            thisLabel.Margin = new Thickness(2, 0, 0, 0);
            thisLabel.Text = text;
            return thisLabel;
        }
    }
}