using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.GameboardPositionHelpers
{
    public class ButtonCP : BaseGraphicsCP
    {
        public string Text { get; set; } = "";
        //public ICommand? Command { get; set; } // will be this specific command.  needs to be saved so it can just execute (regardless of what is really is)


        //public object CommandParameter { get; set; }
        //try with plain command.
        //public ICommand? Command { get; set; } //decided to go ahead and do old fashioned icommand after all.  the board command implements so still okay.

        public Func<bool>? CanExecute { get; set; }

        public Func<Task>? ExecuteAsync { get; set; }

        //public Func<T, Task> ExecuteAsync { get; set; }
       
        public SKColor BorderColor { get; set; } = SKColors.Black;
        public int BorderWidth { get; set; } = 3; // defaults to 3.  can be larger/smaller as needed
        public SKColor FillColor { get; set; } = SKColors.Transparent; // default to transparent
        public int FontSize { get; set; } = 20;
        public SKColor TextColor { get; set; } = SKColors.Black; // can adjust as needed as well
        public bool DidClickButton(SKPoint thisPoint)
        {
            if (CanExecute != null && CanExecute() == false)
            {
                return false;
            }
            
            var thisRect = GetRectangle();
            return MiscHelpers.DidClickRectangle(thisRect, thisPoint);
        }
        private SKRect GetRectangle()
        {
            return SKRect.Create(Location.X, Location.Y, (float)ActualWidth, (float)ActualHeight);
        }
        public ButtonCP()
        {
            OriginalSize = new SKSize(100, 30); // can always adjust as needed.  since its public, then other things can decide it needs the original to be larger
        }
        public override void DrawImage(SKCanvas dc)
        {
            var thisRect = GetRectangle();
            SKPaint thisPaint;
            thisPaint = MiscHelpers.GetSolidPaint(FillColor);
            dc.DrawRect(thisRect, thisPaint);
            thisPaint = MiscHelpers.GetStrokePaint(BorderColor, BorderWidth);
            dc.DrawRect(thisRect, thisPaint);
            thisPaint = MiscHelpers.GetTextPaint(TextColor, FontSize);
            dc.DrawCustomText(Text, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, thisRect, out _);
        }
    }
}