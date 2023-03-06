using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Client.core
{
    internal class Client
    {
        enum MessageCodes : int
        {
            AccForRegistration = 10,
            RegistrationError = 11,
            RegistrationSuccess = 12,

            AccForLogin = 20,
            LoginError = 21,
            LoginSuccess = 22,
        }

        public delegate void MessageHandler(string message);
        MessageHandler messageHandler;
        const string ip = "127.0.0.1";
        const int port = 8081;
        IPEndPoint tcpEndPoint;
        Socket tcpSocket;
        Socket listener;
        public Client()
        {
            tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            System.Threading.Thread.Sleep(1000);
            tcpSocket.Connect(tcpEndPoint);
        }

        public void AddMessageHandler(MessageHandler messageHandler)
        {
            this.messageHandler += messageHandler;
        }
        public void RemoveMessageHandler(MessageHandler messageHandler)
        {
            if (messageHandler != null)
                this.messageHandler -= messageHandler;
        }
        public void Send(string sendMessage)
        {
            var message = sendMessage;
            var data = Encoding.UTF8.GetBytes(message);
            tcpSocket.Send(data);
        }
        public void SendAcccount(string login, string password)
        {
            string toSendString = ((int)MessageCodes.AccForLogin).ToString() + " " + login + " " + password;
            Send(toSendString);
        }

        public void Recieve()
        {
            listener = tcpSocket;
            while (true)
            {
                var buffer = new byte[256];
                int size = 0;
                var answer = new StringBuilder();
                do
                {
                    size = listener.Receive(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (listener.Available > 0);

                if(answer.Length > 0)
                    tryProcessingAnswer(answer);
            }
        }
        public void tryProcessingAnswer(StringBuilder answer)
        {
            try
            {
                ProcessingAnswer(answer);
            }
            catch(ArgumentException ex)
            {
                messageHandler(ex.Message);
            }
        }
        private void ProcessingAnswer(StringBuilder answer)
        {
            var answerStrings = answer.ToString().Split("*/*");
            switch (int.Parse(answerStrings[0]))
            {
                case (int)MessageCodes.LoginSuccess:
                    messageHandler("Вы вошли в аккаунт");
                    break;
                case (int)MessageCodes.LoginError:
                    throw new ArgumentException(answerStrings[1]);
                case (int)MessageCodes.RegistrationSuccess:
                    messageHandler("Аккаунт создан");
                    break;
                case (int)MessageCodes.RegistrationError:
                    throw new ArgumentException(answerStrings[1]);
            }
        }
    }
    
}
