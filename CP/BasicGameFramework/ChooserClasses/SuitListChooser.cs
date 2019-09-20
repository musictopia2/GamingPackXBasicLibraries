using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace BasicGameFramework.ChooserClasses
{
    public class SuitListChooser : IEnumListClass<EnumSuitList>
    {
        CustomBasicList<EnumSuitList> IEnumListClass<EnumSuitList>.GetEnumList()
        {
            return new CustomBasicList<EnumSuitList>()
            { EnumSuitList.Clubs,
            EnumSuitList.Diamonds, EnumSuitList.Hearts, EnumSuitList.Spades};
        }
    }
}