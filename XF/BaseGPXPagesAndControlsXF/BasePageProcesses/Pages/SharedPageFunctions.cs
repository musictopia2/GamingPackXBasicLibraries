using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace BaseGPXPagesAndControlsXF.BasePageProcesses.Pages
{
    public static class SharedPageFunctions
    {
        private static IFontProcesses? _font;
        public static void SendFont(IFontProcesses font)
        {
            _font = font;
        }
        public static Label GetDefaultLabel()
        {
            Label thisLabel = new Label();
            thisLabel.TextColor = Color.Aqua;
            return thisLabel;
        }
        public static Button GetGamingButton(string text, string commandPath)
        {
            Button output = new Button();
            output.TextColor = Color.Black;
            output.BackgroundColor = Color.Aqua;
            if (_font == null)
                _font = Resolve<IFontProcesses>();
            output.FontSize = _font.NormalFontSize;
            output.SetBinding(Button.CommandProperty, new Binding(commandPath));
            output.Text = text;
            output.BorderColor = Color.White;
            output.BorderWidth = 1;
            return output;
        }
        public static Button GetSmallerButton(string text, string commandPath)
        {
            Button output = new Button();
            output.TextColor = Color.Black;
            output.BackgroundColor = Color.Aqua;
            if (_font == null)
                _font = Resolve<IFontProcesses>();
            output.FontSize = _font.SmallFontSize;
            if (_font.HeightRequest > 0)
                output.HeightRequest = _font.HeightRequest;
            output.SetBinding(Button.CommandProperty, new Binding(commandPath));
            output.Text = text;
            return output;
        }
        public static void SetDefaultStartOrientations(View view)
        {
            view.HorizontalOptions = LayoutOptions.Start;
            view.VerticalOptions = LayoutOptions.Start;
            view.Margin = new Thickness(2, 2, 2, 2);
        }
    }
}