using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;

namespace Server.core
{
    class Server
    {
        enum MessageCode
        {
            AccForRegistraation = 0,
            AccForLogin = 1,
            RegistrationError = 10,
            LoginError = 11,
        }
        ApplicationContext context;
        const string ip = "127.0.0.1";
        const int port = 8081;
        IPEndPoint tcpEndPoint;
        Socket tcpSocket;
        Socket listener;
        StringBuilder data;
       public Server()
        {
            context = new ApplicationContext();
            tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(5);
            listener = tcpSocket.Accept();
            data = new StringBuilder();
        }
        public void Start()
        {
            while (true)
            {
                var buffer = new byte[256];
                int size = 0;
                do
                {
                    size = listener.Receive(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (listener.Available > 0);


                string acc = data.ToString();
                CheckRegAccount(acc);
                Console.WriteLine(acc);


                listener.Send(Encoding.UTF8.GetBytes("Успех"));
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }
        }

        void CheckRegAccount(string accountJson)
        {
            var account = JsonSerializer.Deserialize<Account>(accountJson);
            try
            {
                if (IsUnicLogin(account))
                {
                    context.Add(account);
                    context.SaveChanges();
                }
                else throw new ArgumentException("Этот логин уже занят");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                listener.Send(Encoding.UTF8.GetBytes(ex.Message));
            }
        }
        private bool IsUnicLogin(Account account)
        {
            var result = context.Accounts
                .Where(a => a.Login.Equals(account.Login));
            foreach(var r in result)
            {
                Console.WriteLine(r.Login);
            }
            List<Account> accList = result.ToList();
            if (accList.ToList().Count() == 0)
                return false;
            return true;
        }

    }
}
