using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks;
namespace BasicGameFramework.Dice
{
    public interface IRollSingleDice<T> : IAdvancedDIContainer
    {
        Task ShowRollingAsync(CustomBasicList<T> ThisCol);
        Task SendMessageAsync(string category, CustomBasicList<T> thisList);
        Task<CustomBasicList<T>> GetDiceList(string content);
        CustomBasicList<T> RollDice(int howManySections = 6);
    }
}