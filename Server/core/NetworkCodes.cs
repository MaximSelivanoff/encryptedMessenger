using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.core
{
    /// <summary>
    /// Классс для сетевого взаимодействия
    /// </summary>
    static class NetworkCodes
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
        }
        /// <summary>
        /// По входным данным формирует строку для отправки
        /// </summary>
        /// <param name="code">Код сообщения</param>
        /// <param name="paramsToSend">данные для передачи</param>
        /// <returns></returns>
        public static string GetMessage (MessageCodes code, params string[] paramsToSend)
        {
            var builder = new StringBuilder ();
            builder.Append(((int)code).ToString());
            foreach(var msg in paramsToSend)
            {
                builder.Append("*/*");
                builder.Append(msg);
            }
            string message = builder.ToString ();
            return message;
        }
    }
}
