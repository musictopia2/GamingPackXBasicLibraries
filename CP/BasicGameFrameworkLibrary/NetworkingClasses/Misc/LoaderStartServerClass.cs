using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.GlobalClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.SocketClasses;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.GlobalClasses.GlobalDataLoaderClass;
namespace BasicGameFrameworkLibrary.NetworkingClasses.Misc
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