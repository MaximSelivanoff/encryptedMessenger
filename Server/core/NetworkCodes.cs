using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.core
{
    static class NetworkCodes
    {
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
