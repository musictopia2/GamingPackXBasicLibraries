using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace BasicGameFrameworkLibrary.ChooserClasses
{
    public class CardValueListChooser : IEnumListClass<EnumCardValueList>
    {
        CustomBasicList<EnumCardValueList> IEnumListClass<EnumCardValueList>.GetEnumList()
        {
            return new CustomBasicList<EnumCardValueList>()
            {
                EnumCardValueList.LowAce, EnumCardValueList.Two, EnumCardValueList.Three, EnumCardValueList.Four,
                EnumCardValueList.Five, EnumCardValueList.Six, EnumCardValueList.Seven, EnumCardValueList.Eight,
                EnumCardValueList.Nine, EnumCardValueList.Ten, EnumCardValueList.Jack, EnumCardValueList.Queen,
                EnumCardValueList.King
            };
        }
    }
}