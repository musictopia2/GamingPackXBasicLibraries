using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicControlsAndWindowsCore.BasicWindows.BasicConverters;
using BasicControlsAndWindowsCore.Helpers;
using BasicGameFramework.Dice;
using BasicGameFramework.YahtzeeStyleHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace BaseGPXWindowsAndControlsCore.BasicControls.YahtzeeControls
{
    public class ScoresheetWPF<D> : UserControl
            where D : SimpleDice, new()
    {
        public void Init(ScoresheetVM<D> thisMod)
        {
            VisibilityConverter thisCon = new VisibilityConverter();
            thisCon.UseCollapsed = true;
            Binding thisBind = new Binding(nameof(ScoresheetVM<D>.Visible));
            DataContext = thisMod;
            thisBind.Converter = thisCon;
            SetBinding(VisibilityProperty, thisBind);
            Grid thisGrid = new Grid();
            GridHelper.AddAutoColumns(thisGrid, 2);
            // there are 2 groupboxes
            CrossPlatformBorderWPF thisBorder;
            GroupBox thisGroup = new GroupBox();
            thisGroup.VerticalAlignment = VerticalAlignment.Top;
            GridHelper.AddControlToGrid(thisGrid, thisGroup, 0, 0);
            thisGroup.Header = "Top Portion";
            thisGroup.BorderThickness = new Thickness(2, 2, 2, 2);
            thisGroup.Foreground = Brushes.White;
            thisGroup.BorderBrush = Brushes.Black;
            Grid otherGrid = new Grid();
            thisGroup.Content = otherGrid;
            otherGrid.Background = Brushes.White;
            otherGrid.Margin = new Thickness(4, 4, 4, 4);
            GridHelper.AddAutoColumns(otherGrid, 1);
            GridHelper.AddPixelColumn(otherGrid, 75);
            GridHelper.AddPixelColumn(otherGrid, 75);
            GridHelper.AddAutoRows(otherGrid, 9);
            var thisLabel = GetHeaderLabel("Description");
            GridHelper.AddControlToGrid(otherGrid, thisLabel, 0, 0);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, 0, 0);
            thisLabel = GetHeaderLabel("Possible Points");
            GridHelper.AddControlToGrid(otherGrid, thisLabel, 0, 1);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, 0, 1);
            thisLabel = GetHeaderLabel("Points Obtained");
            GridHelper.AddControlToGrid(otherGrid, thisLabel, 0, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, 0, 2);
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
                RowClickerWPF TempRow = new RowClickerWPF();
                GridHelper.AddControlToGrid(otherGrid, TempRow, x, 0);
                TempRow.DataContext = thisRow; //i think.
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
            thisLabel = new TextBlock();
            thisLabel.Background = Brushes.Aqua;
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 1);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 1);
            thisLabel = GetScoreText(bonus);
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 2);
            x += 1; //total score last
            thisLabel = GetFooterText("Total Top Portion");
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
            thisGroup = new GroupBox();
            GridHelper.AddControlToGrid(thisGrid, thisGroup, 0, 1);
            thisGroup.Header = "Bottom Portion";
            thisGroup.BorderThickness = new Thickness(2, 2, 2, 2);
            thisGroup.Foreground = Brushes.White;
            thisGroup.BorderBrush = Brushes.Black;
            otherGrid = new Grid();
            thisGroup.Content = otherGrid;
            otherGrid.Background = Brushes.White;
            otherGrid.Margin = new Thickness(4, 4, 4, 4);
            GridHelper.AddAutoColumns(otherGrid, 1);
            GridHelper.AddPixelColumn(otherGrid, 75);
            GridHelper.AddPixelColumn(otherGrid, 75);
            thisList = (from Items in thisMod.RowList
                        where Items.IsTop == false && Items.RowSection == EnumRowEnum.Regular
                        select Items).ToCustomBasicList();
            GridHelper.AddAutoRows(otherGrid, thisList.Count + 2); // because needs header and footer
            thisLabel = GetHeaderLabel("Description");
            GridHelper.AddControlToGrid(otherGrid, thisLabel, 0, 0);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, 0, 0);
            thisLabel = GetHeaderLabel("Possible Points");
            GridHelper.AddControlToGrid(otherGrid, thisLabel, 0, 1);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, 0, 1);
            thisLabel = GetHeaderLabel("Points Obtained");
            GridHelper.AddControlToGrid(otherGrid, thisLabel, 0, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, 0, 2);
            x = 0;
            foreach (var thisRow in thisList)
            {
                RowClickerWPF tempRow = new RowClickerWPF();
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
                GridHelper.AddControlToGrid(otherGrid, tempRow, x, 0);
                tempRow.Command = thisMod.RowClickedCommand;
                tempRow.DataContext = thisRow;
                Grid.SetColumnSpan(tempRow, 3);
            }
            RowInfo bottomTotal = (from Items in thisMod.RowList
                                   where Items.IsTop == false && Items.RowSection == EnumRowEnum.Totals
                                   select Items).Single();
            x += 1;
            thisLabel = GetFooterText("Total Bottom Portion");
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 0);
            Grid.SetColumnSpan(thisLabel, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 0);
            Grid.SetColumnSpan(thisBorder, 2);
            // Dim TopTotal = (From Items In ThisMod.RowList Where Items.IsTop = True AndAlso Items.RowSection = RowInfo.EnumRowEnum.Totals).Single
            thisLabel = GetScoreText(bottomTotal);
            GridHelper.AddControlToGrid(otherGrid, thisLabel, x, 2);
            thisBorder = GetBorder();
            GridHelper.AddControlToGrid(otherGrid, thisBorder, x, 2);
            Content = thisGrid;
        }
        private void AddBackroundLabel(Grid otherGrid, RowInfo thisRow, int row)
        {
            TextBlock thisText = new TextBlock();
            {
                var withBlock = thisText;
                withBlock.DataContext = thisRow;
                ColorConverter ThisCon = new ColorConverter();
                Binding ThisBind = new Binding(nameof(thisRow.IsRecent));
                ThisBind.Converter = ThisCon;
                withBlock.SetBinding(TextBlock.BackgroundProperty, ThisBind);
                GridHelper.AddControlToGrid(otherGrid, thisText, row, 0);
                Grid.SetColumnSpan(thisText, 3);
            }
        }
        private TextBlock GetFooterText(string text)
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.FontSize = 20;
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            {
                var withBlock = thisLabel;
                withBlock.FontWeight = FontWeights.Bold;
                withBlock.Foreground = Brushes.Black;
                withBlock.Padding = new Thickness((double)3, (double)3, (double)3, (double)3);
                withBlock.TextWrapping = TextWrapping.Wrap;
                withBlock.VerticalAlignment = VerticalAlignment.Center;
                withBlock.Text = text;
            }
            return thisLabel;
        }
        private CrossPlatformBorderWPF GetBorder()
        {
            CrossPlatformBorderWPF thisBorder = new CrossPlatformBorderWPF();
            return thisBorder;
        }
        private TextBlock GetPossibleText(RowInfo thisRow)
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.FontSize = 30; // 30 did not work for making larger text.
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            {
                var withBlock = thisLabel;
                withBlock.Foreground = Brushes.Red;
                withBlock.Padding = new Thickness(3, 3, 3, 3);
                withBlock.VerticalAlignment = VerticalAlignment.Center;
                withBlock.DataContext = thisRow;
                withBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(thisRow.Possible))); // well see
            }
            return thisLabel;
        }
        private TextBlock GetScoreText(RowInfo thisRow)
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.FontSize = 30;
            {
                var withBlock = thisLabel;
                withBlock.Foreground = Brushes.Black;
                withBlock.Padding = new Thickness(3, 3, 3, 3);
                withBlock.VerticalAlignment = VerticalAlignment.Center;
                withBlock.HorizontalAlignment = HorizontalAlignment.Center;
                withBlock.DataContext = thisRow;
                withBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(thisRow.PointsObtained))); // well see
            }
            return thisLabel;
        }
        private TextBlock GetDescriptionText(string text)
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.FontSize = 12;
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            {
                var withBlock = thisLabel;
                withBlock.Foreground = Brushes.Black;
                withBlock.Padding = new Thickness(3, 3, 3, 3);
                withBlock.VerticalAlignment = VerticalAlignment.Center;
                withBlock.Text = text;
            }
            return thisLabel;
        }
        private TextBlock GetHeaderLabel(string text)
        {
            TextBlock thisLabel = new TextBlock();
            thisLabel.FontSize = 15;
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            {
                var withBlock = thisLabel;
                withBlock.FontWeight = FontWeights.Bold;
                withBlock.Foreground = Brushes.Black;
                withBlock.Padding = new Thickness(3, 3, 3, 3);
                withBlock.TextWrapping = TextWrapping.Wrap;
                withBlock.VerticalAlignment = VerticalAlignment.Center;
                withBlock.Text = text;
            }
            return thisLabel;
        }
    }
}