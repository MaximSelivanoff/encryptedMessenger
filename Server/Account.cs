using System;

namespace Server
{
    public class Account
    {
        private int id;
        private string login;
        private string password;
        private void CheckingTheEnteredString(string forCheck, string fieldName)
        {
            var ex = new ArgumentException($"Поле {fieldName} заполнено неверно");

            if (string.IsNullOrWhiteSpace(forCheck))
                throw ex;
            if(forCheck.Contains(' '))
                throw ex;
            if (forCheck.Length < 8)
                throw ex;
        }
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
            get => password;
            set
            {
                CheckingTheEnteredString(value, nameof(Password));
                password = value;
            }
        }
        public Account(int id, string login, string password)
        {
            Login = login;
            Password = password;
            Id = id;
        }
    }
}
