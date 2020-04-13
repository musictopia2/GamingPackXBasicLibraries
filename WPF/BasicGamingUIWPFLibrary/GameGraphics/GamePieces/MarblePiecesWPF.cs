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
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using System.Windows;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.GameGraphics.GamePieces
{
    public class MarblePiecesWPF<E> : BaseGraphicsWPF<MarblePiecesCP<E>>
        where E : struct, Enum
    {
        public static readonly DependencyProperty UseTroubleProperty = DependencyProperty.Register("UseTrouble", typeof(bool), typeof(MarblePiecesWPF<E>), new FrameworkPropertyMetadata(new PropertyChangedCallback(UseTroublePropertyChanged)));
        public bool UseTrouble
        {
            get
            {
                return (bool)GetValue(UseTroubleProperty);
            }
            set
            {
                SetValue(UseTroubleProperty, value);
            }
        }
        private static void UseTroublePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MarblePiecesWPF<E>)sender;
            thisItem.Mains.UseTrouble = (bool)e.NewValue;
        }
    }
}
