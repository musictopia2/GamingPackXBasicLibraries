using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.ViewModels;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace BasicGamingUIXFLibrary.Shells
{
    public abstract class SinglePlayerShellView : BasicGameMainShellView
    {

        //the part for use multiplayer is false is on the bootstrapper level.

        //for this version, don't worry about restore.
        //refer to what i did for the sample game for ideas.
        //see what the patterns are.
        //maybe i don't need abstract after all

        protected Grid? MainGrid;

        public SinglePlayerShellView(
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
            AddAutoRows(MainGrid!, 1);
            AddLeftOverRow(MainGrid!, 1);
            //AddAutoRows(MainGrid!, 2);
        }
        /// <summary>
        /// this adds the part for choosing new game.
        /// </summary>
        /// <param name="game"></param>
        protected virtual void AddNewGame(ParentSingleUIContainer game)
        {
            AddControlToGrid(MainGrid!, game, 0, 0);
        }
        /// <summary>
        /// this will add the part that plays the game but does not care about the button for new game.
        /// </summary>
        /// <param name="game"></param>
        protected virtual void AddMain(ParentSingleUIContainer game)
        {
            AddControlToGrid(MainGrid!, game, 1, 0);
        }

        //protected void AddControlToGrid()

        protected sealed override void PrepUI()
        {
            MainGrid = new Grid(); //was forced to do later because the base class runs first.  by the time it gets to that new, its too late.
            //you can't override this any further.
            OrganizeMainGrid();
            ParentSingleUIContainer newGame = new ParentSingleUIContainer(nameof(SinglePlayerShellViewModel.NewGameVM));
            newGame.HorizontalOptions = LayoutOptions.Start;
            newGame.VerticalOptions = LayoutOptions.Start;
            AddNewGame(newGame);
            ParentSingleUIContainer mains = new ParentSingleUIContainer(nameof(SinglePlayerShellViewModel.MainVM))
            {
                Margin = new Thickness(3)
            };
            AddMain(mains);
        }
        protected sealed override void FinalizeUI()
        {
            Content = MainGrid;
        }


    }
}
