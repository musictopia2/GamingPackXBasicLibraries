using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using Newtonsoft.Json;
using System;
namespace BasicGameFramework.MultiplayerClasses.SavedGameClasses
{
    public class BasicSavedPlainBoardGameClass<E, G, P> : BasicSavedGameClass<P>
        where P : class, IPlayerBoardGame<E>, new()
        where E : struct, Enum
        where G : BaseGraphicsCP, IEnumPiece<E>, new()
    {

        private string _Instructions = "";
        public string Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value))
                {
                    if (ThisMod != null)
                        ThisMod.Instructions = value; //to hook to the view model for notifcations.
                }
            }
        }
        [JsonIgnore]
        public ISimpleBoardVM<E, G, P>? ThisMod;
    }
}