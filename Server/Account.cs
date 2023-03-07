using System;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
    public class Account
    {
        private int id;
        private string login;
        private string passwordHash;
        private DateTime timeStamp;
        public int Id
        {
            get => id;
            set => id = value;
        }
        public string Login
        {
            get => login;
            set
            {
                CheckingTheEnteredString(value, nameof(Login));
                login = value;
            }
        }
        public string Password
        { 
            get => passwordHash;
            set
            {
                //CheckingTheEnteredString(value, nameof(Password));
                passwordHash = value;
            }
        }
        public DateTime TimeStamp
        {
            get => timeStamp;
        }
        public string GetTimestampHash()
        {
            return GetHashMD5(TimeStamp.ToString());
        }
        private void CheckingTheEnteredString(string forCheck, string fieldName)
        {
            var ex = new ArgumentException($"Поле {fieldName} заполнено неверно");

            if (string.IsNullOrWhiteSpace(forCheck))
                //return;
               throw ex;
            if (forCheck.Contains(' '))
                throw ex;
            if (forCheck.Length < 8)
                throw ex;
        }
        public Account(int id, string login, string password)
        {
            Login = login;
            Password = password;
            Id = id;
            timeStamp = DateTime.Now;
        }
        public Account(int id, string login)
        {
            Login = login;
            Password = passwordHash;
            Id = id;
            timeStamp = DateTime.Now;
        }
        public string GetPasswordAndTimeStampHash()
        {
            return GetHashMD5(Password + GetTimestampHash());
        }
        public static string GetHashMD5(string str)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            return Convert.ToBase64String(hash);
        }

    }
}
