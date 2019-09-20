using System.Net.Sockets;
namespace BasicGameFramework.NetworkingClasses.Sockets
{
    internal class ClientInfo
    {
        public TcpClient? Socket { get; set; }
        public NetworkStream? ThisStream { get; set; }
    }
}