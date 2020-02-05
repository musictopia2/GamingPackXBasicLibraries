using BasicGameFramework.NetworkingClasses.Sockets;
using BasicGameFramework.StandardImplementations.CrossPlatform.GlobalClasses;
using static BasicGameFramework.StandardImplementations.CrossPlatform.GlobalClasses.GlobalDataLoaderClass;
namespace BasicGameFramework.NetworkingClasses.Misc
{
    public class LoaderStartServerClass
    {
        readonly bool _isXF;
        private BasicGameServerTCP? _tcpServer;
        public LoaderStartServerClass(bool isXF)
        {
            _isXF = isXF;
        }
        public void PossibleStartServer()
        {
            GlobalDataModel global = Open(_isXF);
            if (global.ServerMode != EnumServerMode.MobileServer)
                return;
            _tcpServer = new BasicGameServerTCP();
            _tcpServer.StartServer(); //hopefully this simple.
        }
    }
}