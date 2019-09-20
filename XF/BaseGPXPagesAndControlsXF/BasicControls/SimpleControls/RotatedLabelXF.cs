using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using Xamarin.Forms;
namespace BaseGPXPagesAndControlsXF.BasicControls.SimpleControls
{
    public class RotatedLabelXF : ContentView
    {
        public RotatedLabelXF(string text)
        {
            StackLayout stack = new StackLayout();
            stack.Spacing = -5;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            stack.Orientation = StackOrientation.Vertical;
            CustomBasicList<char> list = text.ToCustomBasicList();
            list.ForEach(item =>
            {
                Label label = new Label();
                label.HorizontalOptions = LayoutOptions.Start;
                label.VerticalOptions = LayoutOptions.Start;
                label.TextColor = Color.Aqua;
                label.FontAttributes = FontAttributes.Bold;
                label.Text = item.ToString();
                stack.Children.Add(label);
            });
            Content = stack;
        }
    }
}