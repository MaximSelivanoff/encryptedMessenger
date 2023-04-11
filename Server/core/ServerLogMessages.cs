using System;
using System.Security.Policy;

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
        public static string LoginRequestReceiving(string registratedLogin)
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
            string message = $"{DateTime.Now}\nПринят для проверки хэш пароля от пользователя:\nЛогин:{login} \nХэш: {timeStampHash}";
            return message;
        }

        public static string RsaKeyExchangeDataSended(string nonce, string encodedNonceHashString, string N, string e)
        {
            string message = $"{DateTime.Now}\nОтправлены данные Rsa:" +
                                            $"\nСлучайное число:{nonce} " +
                                            $"\nЗашифрованный хэш случайного числа: {encodedNonceHashString}" +
                                            $"\nОткрытый ключ: " +
                                            $"\nN = {N} " +
                                            $"\ne = {e} ";
            return message;
        }
        public static string RsaKeyExchangeDataReceiving(string nonce, string encodedNonceHashString, string N, string e)
        {
            string message = $"{DateTime.Now}\nПолучены ВЕРНЫЕ данные Rsa:" +
                                            $"\nСлучайное число:{nonce} " +
                                            $"\nЗашифрованный хэш случайного числа: {encodedNonceHashString}" +
                                            $"\nОткрытый ключ: " +
                                            $"\nN = {N} " +
                                            $"\ne = {e} ";
            return message;
        }
        public static string RsaKeyExchangeDataReceivingFail(string nonce, string encodedNonceHashString, string N, string e)
        {
            string message = $"{DateTime.Now}\nПолучены НЕВЕРНЫЕ данные Rsa:" +
                                            $"\nСлучайное число:{nonce} " +
                                            $"\nЗашифрованный хэш случайного числа: {encodedNonceHashString}" +
                                            $"\nОткрытый ключ: " +
                                            $"\nN = {N} " +
                                            $"\ne = {e} "
                                            ;
            return message;
        }
        public static string DiffieHellmanExchangeDataSended(string publicKey)
        {
            string message = $"{DateTime.Now}\nОтправлен публичный ключ Диффи-Хеллмана: {publicKey}";
            return message;
        }
        public static string DiffieHellmanExchangeDataRecieving(string otherKey, string sharedKey)
        {
            string message = $"{DateTime.Now}\nПолучен открытый ключ пользователя: {otherKey}" +
                             $"\nОбщий ключ Диффи-Хеллмана сгенерирован: {sharedKey}";
            return message;
        }
    }
}
