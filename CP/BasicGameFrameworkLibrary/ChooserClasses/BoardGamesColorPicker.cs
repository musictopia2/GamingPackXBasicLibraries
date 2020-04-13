using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicGameFrameworkLibrary.ChooserClasses
{
    public class BoardGamesColorPicker<E, G, P> : SimpleEnumPickerVM<E, G>
        where E : struct, Enum
        where G : BaseGraphicsCP, IEnumPiece<E>, new()
        where P : class, IPlayerBoardGame<E>, new()
    {
        public PlayerCollection<P>? PlayerList;
        public void FillInRestOfColors() //this means fill rest of colors is no problem.
        {
            var thisList = ColorsLeft();
            thisList.ShuffleList();
            if (PlayerList.Any(x => x.DidChooseColor == false && x.InGame == true))
                throw new BasicBlankException("There is at least one player who did not choose color who is in game.  That is wrong");
            CustomBasicList<P> tempList = PlayerList.Where(Items => Items.DidChooseColor == false).ToCustomBasicList();
            tempList.ShuffleList();
            //i am guessing in this case, you have to literally have them match.
            if (thisList.Count != tempList.Count)
                throw new BasicBlankException("Does not match.  Therefore, can't populate the rest of the colors");
            tempList.ForEach(thisPlayer =>
            {
                thisPlayer.Color = thisList[tempList.IndexOf(thisPlayer)].EnumValue;
            });
        }
        private CustomBasicCollection<G> ColorsLeft()
        {
            var firstList = PrivateGetList(); //start with entire list
            CustomBasicCollection<G> output = new CustomBasicCollection<G>();
            firstList.ForEach(items =>
            {
                if (AlreadyTaken(items.EnumValue) == false)
                    output.Add(items);
            });
            return output;
        }
        private bool AlreadyTaken(E color)
        {
            return PlayerList.Any(Items => Items.Color.Equals(color));
        }
        public void LoadColors()
        {
            var tempList = ColorsLeft();
            ItemList.ReplaceRange(tempList);
            if (ItemList.Count == 0)
                throw new BasicBlankException("There are no pieces.  Should have already continued to the next step of the game");
            UnselectAll(); //i think

        }
        public BoardGamesColorPicker(CommandContainer command, IEnumListClass<E> choice) : base(command, choice) { }
    }
}
