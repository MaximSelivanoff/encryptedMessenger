using System;
using System.ComponentModel.DataAnnotations;

namespace Server
{
    public class Account
    {
        [Key]
        private int id;
        private string login;
        private string passwordHash;
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
            get => passwordHash;
            set
            {
                CheckingTheEnteredString(value, nameof(Password));
                passwordHash = value;
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
