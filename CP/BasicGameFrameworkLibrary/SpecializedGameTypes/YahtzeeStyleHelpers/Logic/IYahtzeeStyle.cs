using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic
{
    /// <summary>
    /// this is all the unique things that has to be done depending on game.
    /// </summary>
    public interface IYahtzeeStyle
    {
        int BonusAmount(int topScore);
        CustomBasicList<string> GetBottomText { get; }

        //now whoever implements is responsible for using proper information.

        void PopulateBottomScores();
        CustomBasicList<DiceInformation> GetDiceList(); //i think
        void Extra5OfAKind(); //this is the processes for an extra 5 of a kind.
        //will attempt to force somebody to get what is needed themselves.



        bool HasExceptionFor5Kind { get; }//kismet would be true.  others would be false.
    }
}