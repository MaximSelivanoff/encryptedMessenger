using CoreLib;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server.core
{
    internal class Client
    {
        public Socket tcpClient;
        public DiffieHellman Alice;
        public RC4 rc4;
        public bool startChat;
        public Client(Socket tcpClient)
        {
            this.startChat = false;
            this.tcpClient = tcpClient;
        }
        public async Task SendAsync(byte[] data)
        {
            await tcpClient.SendAsync(data);
        }
    }
}