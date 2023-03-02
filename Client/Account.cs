using System;
using System.ComponentModel.DataAnnotations;

namespace Client
{
    public class Account
    {
<<<<<<< HEAD:Client/Account.cs
        public int id;
=======
        [Key]
        private int id;
>>>>>>> parent of 21aafab (Revert "Added MD5 to passwords"):Server/Account.cs
        private string login;
        private string passwordHash;
        private void CheckingTheEnteredString(string forCheck, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(forCheck))
                throw new ArgumentException(fieldName);
            if(forCheck.Contains(' '))
                throw new ArgumentException(fieldName);
            if(forCheck.Length < 8)
                throw new ArgumentException(fieldName);
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
            this.id = id;
        }
    }
}
