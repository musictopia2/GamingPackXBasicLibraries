using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.Shells
{
    public abstract class MultiplayerBasicShellView : BasicGameMainShellView
    {
        public MultiplayerBasicShellView(IGameInfo gameData, 
            BasicData basicData, 
            IStartUp start
            ) : base(gameData, basicData, start)
        {
        }

        //can copy things from the single player shell view.
        //but has other things though.

        protected Grid? MainGrid;
        private TestOptions? _test;

        protected void AddControlToMainGrid(UIElement control, int row, int column)
        {
            AddControlToGrid(MainGrid!, control, row, column);
        }

        protected void AddAutoRowsMainGrid(int howMany)
        {
            AddAutoRows(MainGrid!, howMany);
        }
        protected void AddLeftOverRowsMainGrid(int howMany)
        {
            AddLeftOverRow(MainGrid!, howMany);
        }

        protected void AddAutoColumnsMainGrid(int howMany)
        {
            AddAutoColumns(MainGrid!, howMany);
        }
        protected void AddLeftOverColumnsMainGrid(int howMany)
        {
            AddLeftOverColumn(MainGrid!, howMany);
        }

        /// <summary>
        /// the default has 2 rows but it can be configured if necessary
        /// </summary>
        protected virtual void OrganizeMainGrid()
        {
            if (_test == null)
            {
                throw new BasicBlankException("Never set the test.  Try resolving earlier.  Rethink");
            }
            AddAutoRows(MainGrid!, 1);
            if (_test.ShowNickNameOnShell)
            {
                AddAutoRows(MainGrid!, 1);
            }
            AddLeftOverRow(MainGrid!, 1);
            //AddAutoRows(MainGrid!, 2);
        }
        /// <summary>
        /// this adds the part for choosing new game.
        /// </summary>
        /// <param name="game"></param>
        protected virtual void AddNewGameOrRound(ParentSingleUIContainer game)
        {
            AddControlToGrid(MainGrid!, game, 0, 0);
        }
        /// <summary>
        /// this adds either the part that plays the game or other opening screens like choosing color, etc.
        /// </summary>
        /// <param name="game"></param>
        protected virtual void AddMain(ParentSingleUIContainer game)
        {
            AddControlToGrid(MainGrid!, game, 2, 0);
        }



        protected sealed override void PrepUI()
        {
            _test = Resolve<TestOptions>();
            MainGrid = new Grid(); //was forced to do later because the base class runs first.  by the time it gets to that new, its too late.
            //you can't override this any further.
            OrganizeMainGrid();
            ParentSingleUIContainer newGame = new ParentSingleUIContainer()
            {
                Name = nameof(IBasicMultiplayerShellViewModel.NewGameScreen)
            };
            newGame.HorizontalAlignment = HorizontalAlignment.Left;
            newGame.VerticalAlignment = VerticalAlignment.Top;
            AddNewGameOrRound(newGame);
            //the part for opening can't be overrided
            if (GameData.GameType == EnumGameType.Rounds) //maybe this way is even better.  this should be the pattern.
            {
                ParentSingleUIContainer rounds = new ParentSingleUIContainer()
                {
                    Name = nameof(IBasicMultiplayerShellViewModel.NewRoundScreen)
                };
                rounds.HorizontalAlignment = HorizontalAlignment.Left;
                rounds.VerticalAlignment = VerticalAlignment.Top;
                AddNewGameOrRound(rounds);
            }

            ParentSingleUIContainer mains = new ParentSingleUIContainer()
            {
                Name = nameof(IBasicMultiplayerShellViewModel.MainVM),
                Margin = new Thickness(3)
            };
            AddMain(mains);
            ParentSingleUIContainer opens = new ParentSingleUIContainer()
            {
                Name = nameof(IBasicMultiplayerShellViewModel.OpeningScreen),
                Margin = new Thickness(3)
            };
            AddMain(opens);
            AddOtherStartingScreens();
            if (_test.ShowNickNameOnShell)
            {
                TextBlock text = SharedUIFunctions.GetDefaultLabel();
                text.Name = nameof(IBasicMultiplayerShellViewModel.NickName);
                AddControlToMainGrid(text, 1, 0); //should not happen except the first time anyways.  just needs to prove the first game works.
            }
        }
        /// <summary>
        /// this is used in cases like the board games where it can do the colors.  this can be other options
        /// even for a game like game of life where you have to choose not only colors but gender.
        /// </summary>
        protected virtual void AddOtherStartingScreens() { }
        
        protected sealed override void FinalizeUI()
        {
            Content = MainGrid;
        }


    }
}
