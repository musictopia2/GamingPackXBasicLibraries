using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.NetworkingClasses.Misc;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.GlobalClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace GamePackageSignalRClasses
{
    public class NetworkStartUp : IRegisterNetworks
    {
        private readonly GlobalDataModel _global;
        public NetworkStartUp(GlobalDataModel global)
        {
            _global = global;
        }
        //decided to do the mobile server somewhere else.  so if navigating to another game, no problem.  means the loader has to do it.

        //private BasicGameServerTCP? _tcpServer;
        void IRegisterNetworks.RegisterMultiplayerClasses(GamePackageDIContainer container)
        {
            //will figure out what needs to be registered for network classes.
            if (_global.ServerMode == EnumServerMode.MobileServer)
            {
                //everything required for mobile server.  this means you will listen on a port.  does mean you have to be closed out of the old or runtime errors.
                //_tcpServer = new BasicGameServerTCP();
                //decided to use port 8010 for this time.  hopefully will be okay.
                //_tcpServer.StartServer();
                container.RegisterType<ServerTCPClass>();
                container.RegisterType<TCPDirectSpecificIP>();
                return;
            }
            if (_global.ServerMode == EnumServerMode.LocalHosting)
            {
                container.RegisterType<ClientTCPClass>();
                container.RegisterType<TCPDirectSpecificIP>();
                return;
            }
            container.RegisterType<SignalRMessageService>();

            if (_global.ServerMode == EnumServerMode.AzureHosting)
            {
                container.RegisterType<SignalRAzureEndPoint>();
                return;
            }
            if (_global.ServerMode == EnumServerMode.HomeHosting)
            {
                container.RegisterType<SignalRLocalEndPoint>();
                return;
            }
            throw new BasicBlankException("No hosting found.  Rethinking required");
        }
    }
}