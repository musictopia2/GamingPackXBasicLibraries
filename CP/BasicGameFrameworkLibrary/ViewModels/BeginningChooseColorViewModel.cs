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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.ViewModels
{
    public class BeginningChooseColorViewModel<E, O, P> : Screen, IBlankGameVM, IBeginningColorViewModel, IDisposable
        where E : struct, Enum
        where O : BaseGraphicsCP, IEnumPiece<E>, new()
        where P : class, IPlayerBoardGame<E>, new()
    {
        private readonly BeginningColorModel<E, O, P> _model;
        private readonly IBeginningColorProcesses<E> _processes;


        private string _turn = "";

        public string Turn
        {
            get { return _turn; }
            set
            {
                if (SetProperty(ref _turn, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _instructions = "";

        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        

        //no need for turn information because the shell is handling that part.
        //hint:  needs instructions and whose turn it is.
        //i think its okay if its repeating this time.
        //since its used for a different purpose.

        //public BoardGamesColorPicker<E, O, P> ColorChooser { get; set; }
        public BeginningChooseColorViewModel(CommandContainer commandContainer, BeginningColorModel<E, O, P> model, IBeginningColorProcesses<E> processes)
        {
            CommandContainer = commandContainer;
            _model = model;
            _processes = processes;
            _processes.SetInstructions = (x => Instructions = x);
            _processes.SetTurn = (x => Turn = x); //has to set delegates before init obviously.
            _model.ColorChooser.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
            //hopefully smart enough that for multiplayer that they can't choose if its not their turn (?)
            _model.ColorChooser.ItemClickedAsync += ColorChooser_ItemClickedAsync;
        }
        protected override Task ActivateAsync()
        {
            return _processes.InitAsync();
        }
        private Task ColorChooser_ItemClickedAsync(E piece)
        {
            return _processes.ChoseColorAsync(piece);
        }

        public void Dispose() //hopefully this simple (?)
        {
            _model.ColorChooser.ItemClickedAsync -= ColorChooser_ItemClickedAsync;
        }

        public CommandContainer CommandContainer { get; set; }
    }
}
