namespace BasicGameFramework.BasicEventModels
{
    public enum EnumRepaintCategories
    {
        frombeginning,
        frombasicboard,
        fromothers,
        fromskiasharpboard
    }
    public enum EnumNewCardCategories
    {
        deck, discard, basicmultilepilesinglecard, fromspecialized
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