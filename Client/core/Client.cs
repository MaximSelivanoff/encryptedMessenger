using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client.core
{
    internal class Client
    {
        public static void Start()
        {
            const string ip = "127.0.0.1";
            const int port = 8081;
            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var message = "aooaoao";
            var data = Encoding.UTF8.GetBytes(message);
            tcpSocket.Connect(tcpEndPoint);
            tcpSocket.Send(data);

            var buffer = new byte[256];
            int size = 0;
            var answer = new StringBuilder();
            do
            {
                size = tcpSocket.Receive(buffer);
                answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }
            while (tcpSocket.Available > 0);

            tcpSocket.Shutdown(SocketShutdown.Both);
            tcpSocket.Close();
        }

        public static void Send(string sendMessage)
        {
            const string ip = "127.0.0.1";
            const int port = 8081;
            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var message = sendMessage;
            var data = Encoding.UTF8.GetBytes(message);
            tcpSocket.Connect(tcpEndPoint);
            tcpSocket.Send(data);
            tcpSocket.Shutdown(SocketShutdown.Both);
            tcpSocket.Close();
        }

        public static string Recieve()
        {
            const string ip = "127.0.0.1";
            const int port = 8081;
            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Connect(tcpEndPoint);

            var buffer = new byte[256];
            int size = 0;
            var answer = new StringBuilder();
            do
            {
                size = tcpSocket.Receive(buffer);
                answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }
            while (tcpSocket.Available > 0);

            tcpSocket.Shutdown(SocketShutdown.Both);
            tcpSocket.Close();

            string recieveMessage = answer.ToString();
            return recieveMessage;
        }

    }
}
