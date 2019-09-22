using BasicGameFramework.NetworkingClasses.Data;
namespace GamePackageSignalRClasses
{
    public class CustomEventHandler
    {
        public CustomEventHandler(EnumNetworkCategory category, string message)
        {
            Category = category;
            Message = message;
        }
        public CustomEventHandler(EnumNetworkCategory category)
        {
            Category = category;
            Message = "";
        }
        public string Message { get; } = "";
        public EnumNetworkCategory Category { get; }
    }
}