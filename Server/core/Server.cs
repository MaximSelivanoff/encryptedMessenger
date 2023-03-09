using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using static Server.core.NetworkCodes;

namespace Server.core
{
    class Server
    {
        ApplicationContext context;
        const string ip = "127.0.0.1";
        const int port = 8081;
        IPEndPoint tcpEndPoint;
        Socket tcpSocket;
        Socket listener;
        StringBuilder data;
        public delegate void LogHandler(string message);
        LogHandler logHandler;
        public Server()
        {
            tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(tcpEndPoint);
            context = new ApplicationContext();
        }
        public void Start()
        {
            tcpSocket.Listen();
            listener = tcpSocket.Accept();
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
            string[] dataStrings = data.ToString().Split("*/*");
            int messageCode = int.Parse(dataStrings[0]);
            if (messageCode == (int)MessageCodes.AccForLogin)
            {
                string logMessage = ServerLogMessages.LoginRequestAccepted(dataStrings[1]);
                logHandler(logMessage);

                tryCheckLogin(dataStrings[1]);
            }
            if(messageCode == (int)MessageCodes.LoginPasswordAndTimeStampHash)
            {
                string logMessage = ServerLogMessages.PasswordRequestChecked(dataStrings[1], dataStrings[2]);
                logHandler(logMessage);

                tryCheckPasswordAndTimeStampHash(dataStrings[1], dataStrings[2]);
            }

        }
        public void tryRegistrateAccount(string login, string password)
        {
            int id = 1;
            var accountsList = context.Accounts.ToList();
            if (accountsList.Count > 0)
                id = accountsList[accountsList.Count - 1].Id + 1;
            var account = new Account(id, login, Account.GetHashMD5(password));
            if (IsUnicLogin(account.Login))
            {
                context.Add(account);
                context.SaveChanges();
            }
            else throw new ArgumentException("Этот логин уже занят");
        }
        void tryCheckLogin(string login)
        {
            try
            {
                CheckLogin(login);
            }
            catch (ArgumentException ex)
            {
                var dataString = GetMessage(MessageCodes.LoginError, ex.Message);
                var data = Encoding.UTF8.GetBytes(dataString);
                listener.Send(data);
            }
        }
        private void CheckLogin(string login)
        {
            int id = context.Accounts.ToList().Count + 1;
            if (!IsUnicLogin(login))
            {
                var account = GetAccByLogin(login);
                string timeStampHash = account.GetTimestampHash();

                var dataString = GetMessage(MessageCodes.TimeStampHash, timeStampHash);
                var data = Encoding.UTF8.GetBytes(dataString);
                listener.Send(data);

                string logMessage = ServerLogMessages.TimeStampHashSended(timeStampHash);
                logHandler(logMessage);

                return;
            }
            throw new ArgumentException("Неверный логин");
        }

        private void tryCheckPasswordAndTimeStampHash(string login, string clientPasswordAndTimestampHash)
        {
            try
            {
                string passwordAndTimestampHash = GetAccByLogin(login).GetPasswordAndTimeStampHash();
                if (passwordAndTimestampHash == clientPasswordAndTimestampHash)
                {
                    var dataString = GetMessage(MessageCodes.LoginSuccess);
                    var data = Encoding.UTF8.GetBytes(dataString);
                    listener.Send(data);
                    return;
                }
                throw new ArgumentException("Неверный пароль");
            }
            catch(ArgumentException ex)
            {
                var dataString = GetMessage(MessageCodes.LoginError, ex.Message);
                var data = Encoding.UTF8.GetBytes(dataString);
                listener.Send(data);
            }
        }
        private Account GetAccByLogin(string login)
        {
            var result = context.Accounts
                .Where(a => a.Login == login);
            return result.ToList()[0];
        }
        private bool IsUnicLogin(string login)
        {
            var result = context.Accounts
                .Where(a => a.Login.Equals(login));
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

        public void AddLogHandler(LogHandler newHandler)
        {
            logHandler += newHandler;
        }
        public void RemoveLogHandler(LogHandler newHandler)
        {
            if(logHandler!= null)
                logHandler -= newHandler;
        }
    }
}
