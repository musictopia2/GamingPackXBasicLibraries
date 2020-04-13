using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders.AttachedProperties;
namespace BasicGamingUIXFLibrary.Helpers
{
    public static class SharedUIFunctions
    {

        //public static bool BoardProcessing { get; set; }

        private static IFontProcesses? _font;
        public static void SendFont(IFontProcesses font)
        {
            _font = font;
        }
        public static Label GetDefaultLabel()
        {
            Label output = new Label();
            output.TextColor = Color.Aqua;
            return output;
        }
        public static Button GetGamingButton(string text, string name)
        {
            Button output = new Button();
            //the good news is since they now show the color as black, may not have to set the color anymore.
            //output.TextColor = Color.Black;
            output.BackgroundColor = Color.Aqua;
            if (_font == null)
                _font = Resolve<IFontProcesses>();
            output.FontSize = _font.NormalFontSize;
            output.SetName(name); //hopefully this simple.
            output.Text = text;
            output.BorderColor = Color.White;
            output.BorderWidth = 1;
            return output;
        }
        public static Button GetSmallerButton(string text, string name)
        {
            Button output = new Button();
            //output.TextColor = Color.Black;
            output.BackgroundColor = Color.Aqua;
            if (_font == null)
                _font = Resolve<IFontProcesses>();
            output.FontSize = _font.SmallFontSize;
            if (_font.HeightRequest > 0)
                output.HeightRequest = _font.HeightRequest;
            output.SetName(name); //hopefully this simple.
            output.Text = text;
            return output;
        }
        public static void SetDefaultStartOrientations(View view)
        {
            view.HorizontalOptions = LayoutOptions.Start;
            view.VerticalOptions = LayoutOptions.Start;
            view.Margin = new Thickness(2, 2, 2, 2);
        }

        public static void AddVerticalLabelGroup(string header, string name, StackLayout stack, bool boldHeader = false)
        {
            Label thisLabel = GetDefaultLabel();
            thisLabel.Text = $"{header}:";
            if (boldHeader == true)
                thisLabel.FontAttributes = FontAttributes.Bold;
            stack.Children.Add(thisLabel);
            thisLabel = GetDefaultLabel();
            thisLabel.SetBinding(Label.TextProperty, name); //unfortunately, this has no ability to wrap.  has to see what i did before in order to resolve this.
            stack.Children.Add(thisLabel);
        }

    }
}
