using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows;

namespace Server.core
{
    class Server
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
        ApplicationContext context;
        const string ip = "127.0.0.1";
        const int port = 8081;
        IPEndPoint tcpEndPoint;
        Socket tcpSocket;
        Socket listener;
        StringBuilder data;
        public Server()
        {
            tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(tcpEndPoint);
        }
        public void Start()
        {
            tcpSocket.Listen();
            listener = tcpSocket.Accept();
            context = new ApplicationContext();
            while (true)
            {
                data = new StringBuilder();
                var buffer = new byte[256];
                int size = 0;
                do
                {
                    size = listener.Receive(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (listener.Available > 0);

                DataProcessing(data);
            }
        }

        void DataProcessing(StringBuilder data)
        {
            string[] dataStrings = data.ToString().Split(" ");
            int messageCode = int.Parse(dataStrings[0]);
            if (messageCode == (int)MessageCodes.AccForLogin)
            {
                tryLogAccount(dataStrings[1], GetHashMD5(dataStrings[2]));
            }

        }
        public void tryRegistrateAccount(string login, string password)
        {

            var accountsList = context.Accounts.ToList();
            int id = accountsList[accountsList.Count- 1].Id + 1;
            var account = new Account(id, login, GetHashMD5(password));
            if (IsUnicLogin(account))
            {
                context.Add(account);
                context.SaveChanges();

                var dataString = ((int)MessageCodes.RegistrationSuccess).ToString();
                var data = Encoding.UTF8.GetBytes(dataString);
                listener.Send(data);

            }
            else throw new ArgumentException("Этот логин уже занят");
        }
        void tryLogAccount(string login, string passwordHash)
        {
            try
            {
                LogAccount(login, passwordHash);
            }
            catch (ArgumentException ex)
            {
                var dataString = (int)MessageCodes.LoginError + "*/*" + ex.Message;
                var data = Encoding.UTF8.GetBytes(dataString);
                listener.Send(data);
            }
        }
        private void LogAccount(string login, string passwordHash)
        {
            int id = context.Accounts.ToList().Count + 1;
            var account = new Account(id, login, passwordHash);
            if (!IsUnicLogin(account))
            {
                if (IsCorrectPassword(account))
                {
                    var dataString = ((int)MessageCodes.LoginSuccess).ToString();
                    var data = Encoding.UTF8.GetBytes(dataString);
                    listener.Send(data);
                    return;
                }
            }
            throw new ArgumentException("Неверный логин или пароль");
        }

        private bool IsUnicLogin(Account account)
        {
            var result = context.Accounts
                .Where(a => a.Login.Equals(account.Login));
            foreach (var r in result)
            {
                Console.WriteLine(r.Login);
            }
            List<Account> accList = result.ToList();
            if (accList.ToList().Count() == 0)
                return true;
            return false;
        }

        private bool IsCorrectPassword(Account account)
        {
            var result = context.Accounts.Select(a => a).
                 Where(a => a.Login.
                 Equals(account.Login)).
                 Where(a => a.Password.
                 Equals(account.Password));
            if (result.ToList().Count == 0)
                return false;
            else
                return true;
        }

        static string GetHashMD5(string str)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            return Convert.ToBase64String(hash);
        }
    }
}
