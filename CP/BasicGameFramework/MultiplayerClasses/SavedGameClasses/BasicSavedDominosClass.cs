using BasicGameFramework.Dominos;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace BasicGameFramework.MultiplayerClasses.SavedGameClasses
{
    public class BasicSavedDominosClass<D, P> : BasicSavedGameClass<P>
        where D : IDominoInfo, new()
        where P : class, IPlayerSingleHand<D>, new()
    {
        public SavedScatteringPieces<D>? BoneYardData { get; set; }
    }
}