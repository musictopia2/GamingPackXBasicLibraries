using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace BasicGamingUIXFLibrary.Shells
{
    public abstract class MultiplayerBasicShellView : BasicGameMainShellView
    {


        //can copy things from the single player shell view.
        //but has other things though.

        protected Grid? MainGrid;
        private TestOptions? _test;

        public MultiplayerBasicShellView(
            IGamePlatform customPlatform,
            IGameInfo gameData,
            BasicData basicData,
            IStartUp start,
            IStandardScreen screen) : base(customPlatform, gameData, basicData, start, screen)
        {
        }

        protected void AddControlToMainGrid(View control, int row, int column)
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
            if (_test!.ShowNickNameOnShell)
            {
                AddControlToGrid(MainGrid!, game, 2, 0);
            }
            else
            {
                AddControlToGrid(MainGrid!, game, 1, 0);
            }
        }



        protected sealed override void PrepUI()
        {
            _test = Resolve<TestOptions>();
            MainGrid = new Grid(); //was forced to do later because the base class runs first.  by the time it gets to that new, its too late.
            //you can't override this any further.
            OrganizeMainGrid();
            ParentSingleUIContainer newGame = new ParentSingleUIContainer(nameof(IBasicMultiplayerShellViewModel.NewGameScreen));
            newGame.HorizontalOptions = LayoutOptions.Start;
            newGame.VerticalOptions = LayoutOptions.Start;
            AddNewGameOrRound(newGame);
            //the part for opening can't be overrided
            if (GameData.GameType == EnumGameType.Rounds) //maybe this way is even better.  this should be the pattern.
            {
                ParentSingleUIContainer rounds = new ParentSingleUIContainer(nameof(IBasicMultiplayerShellViewModel.NewRoundScreen));
                rounds.HorizontalOptions = LayoutOptions.Start;
                rounds.VerticalOptions = LayoutOptions.Start;
                AddNewGameOrRound(rounds);
            }

            ParentSingleUIContainer mains = new ParentSingleUIContainer(nameof(IBasicMultiplayerShellViewModel.MainVM))
            {
                Margin = new Thickness(3)
            };
            AddMain(mains);
            ParentSingleUIContainer opens = new ParentSingleUIContainer(nameof(IBasicMultiplayerShellViewModel.OpeningScreen))
            {
                Margin = new Thickness(3)
            };
            AddMain(opens);
            AddOtherStartingScreens();
            if (_test.ShowNickNameOnShell)
            {
                Label text = SharedUIFunctions.GetDefaultLabel();
                text.SetName(nameof(IBasicMultiplayerShellViewModel.NickName));
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
