using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using SkiaSharp.Views.WPF;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.MiscClasses;
using SkiaSharp;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.BasicControls.GameBoards
{
    public class SkiaSharpGameBoard : SKElement, ISkiaSharpGameBoard
    {
        public event SingleClickBoardEventHandler? SingleClickBoard;
        public event CPPaintEventHandler? CPPaint;
        public void StartClick(double x, double y)
        {
            SingleClickBoard?.Invoke(x, y);
        }
        public void StartInvalidate(SKCanvas canvas, double width, double height)
        {
            Execute.OnUIThread(() =>
            {
                CPPaint?.Invoke(canvas, width, height);
            });
        }
    }
}
