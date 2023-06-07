using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreLib;
using System.Numerics;
using System.Net.Http;

namespace Server.core
{
    public class Server
    {
        ApplicationContext context;
        const string ip = "127.0.0.1";
        const int port = 8081;
        IPEndPoint tcpEndPoint;
        Socket tcpListener;
        Socket tcpClient;
        StringBuilder data;
        public delegate void LogHandler(string message);
        public LogHandler logHandler;
        DiffieHellman Alice;
        RC4 rc4;
        bool startChat;

        public Server()
        {
            tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpListener.Bind(tcpEndPoint);
            context = new ApplicationContext();
            startChat = false;
        }
        public async Task StartAsync()
        {
            tcpListener.Listen();

            while (true)
            {
                tcpClient = await tcpListener.AcceptAsync();
                await ClientProcessing(tcpClient);
            }
        }
        async Task ClientProcessing(Socket tcpClient)
        {
            while (true)
            {
                data = new StringBuilder();
                var buffer = new byte[256];
                int size = 0;
                do
                {
                    size = await tcpClient.ReceiveAsync(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (tcpClient.Available > 0);
                await DataProcessing(data, tcpClient);
            }
            tcpClient.Shutdown(SocketShutdown.Both);
            tcpClient.Close();
        }
        async Task DataProcessing(StringBuilder data, Socket tcpClient)
        {
            string[] dataStrings = data.ToString().Split("*/*");
            int messageCode = int.Parse(dataStrings[0]);
            if (messageCode == (int)NetworkCodes.MessageCodes.AccForLogin)
            {
                string logMessage = ServerLogMessages.LoginRequestReceiving(dataStrings[1]);
                logHandler(logMessage);

                var returnData = tryCheckLogin(dataStrings[1]);

                await tcpClient.SendAsync(returnData);
            }
            if (messageCode == (int)NetworkCodes.MessageCodes.LoginPasswordAndTimeStampHash)
            {
                string logMessage = ServerLogMessages.PasswordRequestChecked(dataStrings[1], dataStrings[2]);
                logHandler(logMessage);
                var returnData = tryCheckPasswordAndTimeStampHash(dataStrings[1], dataStrings[2]);

                await tcpClient.SendAsync(returnData);


                var returnString = Encoding.UTF8.GetString(returnData, 0, returnData.Length);
                string[] toCheckData = returnString.Split("*/*");

                int toCheckCode = int.Parse(toCheckData[0].ToString());
                if (toCheckCode == (int)NetworkCodes.MessageCodes.LoginSuccess)
                {
                    returnData = Encoding.UTF8.GetBytes(RsaKeyExchangeMessageGen());
                    await tcpClient.SendAsync(returnData);
                }

                await tcpClient.SendAsync(returnData);
            }
            if (messageCode == (int)NetworkCodes.MessageCodes.RsaKeyExchange)
            {
                bool startDiffieHellman;
                var returnData = RsaKeyProcessing(dataStrings[1], dataStrings[2], dataStrings[3], dataStrings[4], out startDiffieHellman);
                await tcpClient.SendAsync(returnData);
                if(startDiffieHellman)
                {
                    Alice = new DiffieHellman();
                    var dataString = DiffieHellmanExchange.GenKeyMessForClient(Alice, logHandler);
                    returnData = Encoding.UTF8.GetBytes(dataString);
                    await tcpClient.SendAsync(returnData);
                }
            }
            if(messageCode == (int)NetworkCodes.MessageCodes.DiffieHellmanExchange)
            {
                var returnData = DiffieHellmanProcessing(dataStrings[1], out startChat, out var key);
                await tcpClient.SendAsync(returnData);
                if(startChat)
                {
                    rc4 = new RC4(Encoding.UTF8.GetBytes(key.ToString()));
                }
            }
            if (messageCode == (int)NetworkCodes.MessageCodes.ChatMessage)
            {
                ChatGet(dataStrings[1]);
            }

        }

        string RsaKeyExchangeMessageGen()
        {
            return RsaKeyExchange.GenKeyMessForClient(logHandler);
        }
        private byte[] RsaKeyProcessing(string nonce, string encodedNonceHashString, string N, string e, out bool doNextStep)
        {
            // сравнение полученного сообщения и вычисленного
            var nonseHashForCheck = Account.GetHashMD5(nonce);
            var dataForCheck = Rsa.EncodeWithEnAlph(nonseHashForCheck, BigInteger.Parse(e), BigInteger.Parse(N));
            var encodedForCheck = Rsa.EncodeToString(dataForCheck);
            string dataString;
            if (encodedForCheck == encodedNonceHashString)
            {
                doNextStep = true;
                logHandler(ServerLogMessages.RsaKeyExchangeDataReceiving(nonce, encodedNonceHashString, N, e));
                dataString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.RsaKeyExchangeSuccess);
            }
            else
            {
                doNextStep  = false;
                logHandler(ServerLogMessages.RsaKeyExchangeDataReceivingFail(nonce, encodedNonceHashString, N, e));
                dataString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.RsaKeyExchangeError);
            }
            var data = Encoding.UTF8.GetBytes(dataString);
            return data;
        }
        private byte[] DiffieHellmanProcessing(string otherPublicKey, out bool startNext, out BigInteger keyForRc4)
        {
            var secret = Alice.GenerateSharedSecret(BigInteger.Parse(otherPublicKey));
            var dataString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.DiffieHellmanExchangeSuccess);
            var data = Encoding.UTF8.GetBytes(dataString);
            logHandler(ServerLogMessages.DiffieHellmanExchangeDataRecieving(otherPublicKey, secret.ToString()));
            startNext = true;
            keyForRc4 = secret;
            return data;
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
        byte[] tryCheckLogin(string login)
        {
            try
            {
                var data = CheckLogin(login);
                return data;
            }
            catch (ArgumentException ex)
            {
                var dataString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.LoginError, ex.Message);
                var data = Encoding.UTF8.GetBytes(dataString);
                return data;
            }
        }
        private byte[] CheckLogin(string login)
        {
            int id = context.Accounts.ToList().Count + 1;
            if (!IsUnicLogin(login))
            {
                var account = GetAccByLogin(login);
                string timeStampHash = account.GetTimestampHash();

                var dataString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.TimeStampHash, timeStampHash);
                var data = Encoding.UTF8.GetBytes(dataString);

                string logMessage = ServerLogMessages.TimeStampHashSended(timeStampHash);
                logHandler(logMessage);

                return data;
            }
            throw new ArgumentException("Неверный логин");
        }
        private byte[] tryCheckPasswordAndTimeStampHash(string login, string clientPasswordAndTimestampHash)
        {
            try
            {
                string passwordAndTimestampHash = GetAccByLogin(login).GetPasswordAndTimeStampHash();
                if (passwordAndTimestampHash == clientPasswordAndTimestampHash)
                {
                    var dataString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.LoginSuccess);
                    var data = Encoding.UTF8.GetBytes(dataString);
                    return data;
                }
                throw new ArgumentException("Неверный пароль");
            }
            catch(ArgumentException ex)
            {
                var dataString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.LoginError, ex.Message);
                var data = Encoding.UTF8.GetBytes(dataString);
                return data;
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

        public void ChatGet(string message)
        {
            var byteMessage = Encoding.UTF8.GetBytes(message);
            logHandler(ServerLogMessages.GetChatMessage(message));
            logHandler($"Расшифровка: {Encoding.UTF8.GetString(rc4.Decrypt(byteMessage))}");
        }
        public async void ChatSend(string message)
        {
            if (!startChat)
                return;
            var chiperMessage = rc4.Encrypt(Encoding.UTF8.GetBytes(message));
            var netMess = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.ChatMessage, Encoding.UTF8.GetString(chiperMessage));
            logHandler(ServerLogMessages.SendChatMessage(Encoding.UTF8.GetString(chiperMessage)));
            logHandler($"Расшифровка: {message}");
            await tcpClient.SendAsync(Encoding.UTF8.GetBytes(netMess));
        }
    }
}
