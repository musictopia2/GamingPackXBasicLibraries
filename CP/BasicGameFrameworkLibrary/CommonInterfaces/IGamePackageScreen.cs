namespace BasicGameFrameworkLibrary.CommonInterfaces
{
    /// <summary>
    /// had to change the name because iscreen was already used on the cross platform that does not even necessarily apply to game package.
    /// </summary>
    public interface IGamePackageScreen
    {
        void CalculateScreens(); //decided to make xamarin forms implement it after all.  if it decides to ignore in test mode, they have that right too.
    }
}