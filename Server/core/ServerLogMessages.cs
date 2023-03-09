using System;

namespace Server
{
    static internal class ServerLogMessages
    {
        public static string LoginRequestAccepted(string registratedLogin)
        {
            string message = $"{DateTime.Now}\nПринят запрос на аутентификацию пользователя {registratedLogin}";
            return message;
        }
        public static string TimeStampHashSended(string timeStampHash)
        {
            string message = $"{DateTime.Now}\nОтправлен хэш временной метки: {timeStampHash}";
            return message;
        }
        public static string PasswordRequestChecked(string login, string timeStampHash)
        {
            string message = $"{DateTime.Now}\nПринят для проверки хэш пароля от пользователя:\nЛогин:{login} \nХэш: {timeStampHash}";
            return message;
        }
    }
}
