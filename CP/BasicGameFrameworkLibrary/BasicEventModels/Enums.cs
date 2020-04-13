namespace BasicGameFrameworkLibrary.BasicEventModels
{
    public enum EnumRepaintCategories //decided to make them proper casing.
    {
        FromBeginning,
        FromBasicboard,
        FromOthers,
        FromSkiasharpboard
    }
    public enum EnumNewCardCategories
    {
        Deck, Discard, Basicmultilepilesinglecard, FromSpecialized
    }
    public enum EnumAnimcationDirection
    {
        StartUpToCard = 1,
        StartDownToCard = 2, // if there are others, rethink
        StartCardToUp = 3,
        StartCardToDown = 4
    }
    public enum EnumOptionChosen
    {
        Yes = 1,
        No = 2
    }
    public enum EnumSocketCategory
    {
        None, Client, Server
    }
}