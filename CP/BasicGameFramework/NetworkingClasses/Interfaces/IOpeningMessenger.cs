﻿using System.Threading.Tasks;
namespace BasicGameFramework.NetworkingClasses.Interfaces
{
    public interface IOpeningMessenger
    {
        Task ConnectedToHostAsync(IMessageChecker thisCheck, string hostName);
        Task HostNotFoundAsync(IMessageChecker thisCheck);
        Task HostConnectedAsync(IMessageChecker thisCheck);
    }
}