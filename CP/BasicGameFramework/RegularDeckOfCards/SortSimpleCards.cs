using BasicGameFramework.BasicDrawables.Interfaces;
using System;
namespace BasicGameFramework.RegularDeckOfCards
{
    public class SortSimpleCards<R> : ISortObjects<R>
        where R : IRegularCard, new()
    {
        public EnumSortCategory SuitForSorting { get; set; }
        public int Compare(R x, R y)
        {
            switch (SuitForSorting)
            {
                case EnumSortCategory.NumberSuit:
                    {
                        if (x.Value < y.Value)
                            return -1;
                        else if (x.Value > y.Value)
                            return 1;
                        else
                        {
                            if (x.Suit < y.Suit)
                                return -1;
                            else if (x.Suit > y.Suit)
                                return 1;
                            return 0;
                        }
                    }
                case EnumSortCategory.SuitNumber:
                    {
                        if (x.Suit < y.Suit)
                            return -1;
                        else if (x.Suit > y.Suit)
                            return 1;
                        else if (x.Value < y.Value)
                            return -1;
                        else if (x.Value > y.Value)
                            return 1;
                        return 0;
                    }
                default:
                    {
                        throw new Exception("Sorting Not Supported.  Try creating a new class and replacing registration");
                    }
            }
        }
    }
}