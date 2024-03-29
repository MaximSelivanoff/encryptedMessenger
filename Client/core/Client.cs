﻿using Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using CoreLib;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Linq;
using static Server.core.Server;

namespace Client.core
{
    internal class Client
    {
        public delegate void MessageHandler(string message);
        MessageHandler messageHandler;
        const string ip = "127.0.0.1";
        const int port = 8081;
        IPEndPoint tcpEndPoint;
        Socket tcpSocket;
        Socket listener;
        DiffieHellman Bob;
        public delegate void ChatMessagesHandler(string message);
        ChatMessagesHandler chatHandler;
        BigInteger keyForRc4;

        RC4 rc4;

        string clientLogin;
        string clientPassword;

        public bool ChatIsActive { get; private set; }

        public Client()
        {
            tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            System.Threading.Thread.Sleep(1000);
            tcpSocket.Connect(tcpEndPoint);

            ChatIsActive = false;
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
            var data = Encoding.UTF8.GetBytes(sendMessage);
            tcpSocket.Send(data);
        }
        public void SendAcccount(string login)
        {
            string toSendString = ((int)NetworkCodes.MessageCodes.AccForLogin).ToString() + "*/*" + login;
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

                if (answer.Length > 0)
                    tryProcessingAnswer(answer);
            }
        }
        public void tryProcessingAnswer(StringBuilder answer)
        {
            try
            {
                ProcessingAnswer(answer);
            }
            catch (ArgumentException ex)
            {
                messageHandler(ex.Message);
            }
        }
        private void ProcessingAnswer(StringBuilder answer)
        {
            var answerStrings = answer.ToString().Split("*/*");
            switch (int.Parse(answerStrings[0]))
            {
                case (int)NetworkCodes.MessageCodes.LoginSuccess:
                    messageHandler("Вы вошли в аккаунт");
                    break;
                case (int)NetworkCodes.MessageCodes.LoginError:
                    throw new ArgumentException(answerStrings[1]);
                case (int)NetworkCodes.MessageCodes.RegistrationSuccess:
                    messageHandler("Аккаунт создан");
                    break;
                case (int)NetworkCodes.MessageCodes.RegistrationError:
                    throw new ArgumentException(answerStrings[1]);
                case (int)NetworkCodes.MessageCodes.TimeStampHash:
                    SendLoginPasswordAndTimeStampHash(answerStrings[1]);
                    break;
                case (int)NetworkCodes.MessageCodes.RsaKeyExchange:
                    RsaKeyProcessing(answerStrings[1], answerStrings[2], answerStrings[3], answerStrings[4]);
                    break;
                case (int)NetworkCodes.MessageCodes.DiffieHellmanExchange:
                    DiffieHellmanProcessing(answerStrings[1], answerStrings[2],answerStrings[3], out keyForRc4);
                    messageHandler("Получен запрос на соединение по Диффи-Хеллману");
                    break;
                case (int)NetworkCodes.MessageCodes.DiffieHellmanExchangeSuccess:
                    messageHandler("Обмен ключами по протоолу Диффи-Хеллмана прошёл успешно");
                    ActivateChat(keyForRc4.ToString());
                    break;
                case (int)NetworkCodes.MessageCodes.DiffieHellmanExchangeError:
                    messageHandler("Обмен ключами по протоколу Диффи-Хеллмана не удался");
                    break;
                case (int)NetworkCodes.MessageCodes.ChatMessage:
                    ChatGet(answerStrings[1]);
                    break;
            }
        }

        private void DiffieHellmanProcessing(string otherKey, string otherPrime,string otherGenerator, out BigInteger keyForRc4)
        {
            Bob = new DiffieHellman(BigInteger.Parse(otherPrime), BigInteger.Parse(otherGenerator));
            keyForRc4 = Bob.GenerateSharedSecret(BigInteger.Parse(otherKey));
            var publicKey = Bob.PublicKey.ToString();
            var resultString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.DiffieHellmanExchange, publicKey);
            Send(resultString);
            
        }

        private void RsaKeyProcessing(string nonce, string encodedNonceHashString, string N, string e)
        {
            // сравнение полученного сообщения и вычисленного
            var nonseHashForCheck = Account.GetHashMD5(nonce);
            var dataForCheck = Rsa.EncodeWithEnAlph(nonseHashForCheck, BigInteger.Parse(e), BigInteger.Parse(N));
            var encodedForCheck = Rsa.EncodeToString(dataForCheck);
            if (encodedForCheck == encodedNonceHashString)
            {
                // вычисление сообщения для сервера
                var nonceClient = CryptoAlgorithms.GenerateBigInt(256);
                var nonceHashCient = Account.GetHashMD5(nonceClient.ToString());
                var rsaClient = new Rsa(256);
                var encodedNonceHashClient = rsaClient.Encode(nonceHashCient);
                var encodedNonceHashStringClient = Rsa.EncodeToString(encodedNonceHashClient);

                (var NClient, var eClient) = rsaClient.GetPublicKey();
                var resultString = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.RsaKeyExchange,
                                                       nonceClient.ToString(),
                                                       encodedNonceHashStringClient,
                                                       NClient.ToString(),
                                                       eClient.ToString());
                Send(resultString);
            }
        }
        private void SendLoginPasswordAndTimeStampHash(string TimeStampHash)
        {
            string PasswordAndTimeStampHash = GetHashMD5(GetHashMD5(clientPassword) + TimeStampHash);
            string toSendString = (int)NetworkCodes.MessageCodes.LoginPasswordAndTimeStampHash
                                  + "*/*" + clientLogin
                                  + "*/*" + PasswordAndTimeStampHash;
            Send(toSendString);
        }
        public static string GetHashMD5(string str)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            return Convert.ToBase64String(hash);
        }
        public void SetLoginAndPassword(string login, string password)
        {
            clientLogin = login;
            clientPassword = password;
        }

        void ActivateChat(string key)
        {
            ChatIsActive = true;
            var byteKey = Encoding.UTF8.GetBytes(key);
            rc4 = new RC4(byteKey);
        }
        public void ChatSend(string message)
        {
            var chiperMessage = rc4.Encrypt(Encoding.UTF8.GetBytes(message));
            var netMess = NetworkCodes.GetMessage(NetworkCodes.MessageCodes.ChatMessage, Encoding.UTF8.GetString(chiperMessage));
            Send(netMess);

            var binMess = string.Join("", message.Select(b => Convert.ToString(b, 2)));
            var binEncodedMess = string.Join("", chiperMessage.Select(b => Convert.ToString(b, 2)));

            chatHandler($"Отправлено в {DateTime.Now}:\n{message}\n" +
                $"Бинарное представление сообщения: {binMess}\n" +
            $"Бинарное представление зашифрованного сообщения:\n{binEncodedMess}\n\n");
        }
        public void ChatGet(string chiperMessage)
        {

            var message = rc4.Decrypt(Encoding.UTF8.GetBytes(chiperMessage));

            var binMess = string.Join("", message.Select(b => Convert.ToString(b, 2)));
            var binEncodedMess = string.Join("", chiperMessage.Select(b => Convert.ToString(b, 2)));

            chatHandler($"Получено в {DateTime.Now}:\n{Encoding.UTF8.GetString(message)}\n" +
                $"Бинарное представление сообщения: {binMess}\n" +
            $"Бинарное представление полученного зашифрованного сообщения:\n{binEncodedMess}\n\n");
        }
        public void AddChatHandler(ChatMessagesHandler newHandler)
        {
            chatHandler += newHandler;
        }
        public void RemoveChatHandler(ChatMessagesHandler newHandler)
        {
            if (chatHandler != null)
                chatHandler -= newHandler;
        }
    }

}
