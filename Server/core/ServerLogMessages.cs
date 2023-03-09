using System;

namespace Server
{
/// <summary>
/// Класс для логов сервера
/// </summary>
    static internal class ServerLogMessages
    {
        /// <summary>
        /// Генерирует лог при запросе на аутентификацию
        /// </summary>
        /// <param name="registratedLogin"></param>
        /// <returns>Лог запроса аутентифиации по логину</returns>
        public static string LoginRequestAccepted(string registratedLogin)
        {
            string message = $"{DateTime.Now}\nПринят запрос на аутентификацию пользователя {registratedLogin}";
            return message;
        }
        /// <summary>
        /// Генерирует лог при отправке хэша временной метки
        /// </summary>
        /// <param name="timeStampHash"></param>
        /// <returns>Хэш временной метки</returns>
        public static string TimeStampHashSended(string timeStampHash)
        {
            string message = $"{DateTime.Now}\nОтправлен хэш временной метки: {timeStampHash}";
            return message;
        }
        /// <summary>
        /// Генерирует лог принятия хэша от пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="timeStampHash"></param>
        /// <returns></returns>
        public static string PasswordRequestChecked(string login, string timeStampHash)
        {
            string message = $"{DateTime.Now}\nПринят для проверки хэш пароля от пользователя:\nЛогин:{login} \nХэш: {timeStamЫpHash}";
            return message;
        }
    }
}
