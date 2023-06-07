using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    /// <summary>
    /// Классс для сетевого взаимодействия
    /// </summary>
    public static class NetworkCodes
    {
        /// <summary>
        /// Коды для отправки сообщений по сети
        /// </summary>
        public enum MessageCodes : int
        {
            AccForRegistration = 10,
            RegistrationError = 11,
            RegistrationSuccess = 12,

            AccForLogin = 20,
            LoginError = 21,
            LoginSuccess = 22,

            TimeStampHash = 30,
            LoginPasswordAndTimeStampHash = 31,

            RsaKeyExchange = 40,
            RsaKeyExchangeError = 41,
            RsaKeyExchangeSuccess = 42,

            DiffieHellmanExchange = 50,
            DiffieHellmanExchangeError = 51,
            DiffieHellmanExchangeSuccess = 52,

            ChatMessage = 60

        }
        /// <summary>
        /// По входным данным формирует строку для отправки
        /// </summary>
        /// <param name="code">Код сообщения</param>
        /// <param name="paramsToSend">данные для передачи</param>
        /// <returns></returns>
        /// 
        public static string GetMessage (MessageCodes code, params string[] paramsToSend)
        {
            var builder = new StringBuilder ();
            builder.Append(((int)code).ToString());
            foreach(var msg in paramsToSend)
            {
                builder.Append("*/*");
                builder.Append(msg);
            }
            builder.Append("*/*");
            string message = builder.ToString ();
            return message;
        }
    }
}
