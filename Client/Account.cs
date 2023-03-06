using System;

namespace Client
{
    public class Account
    {
        private int id;
>>>>>>> parent of 21aafab (Revert "Added MD5 to passwords"):Server/Account.cs
        private string login;
        private string password;
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
            this.id = id;
        }
    }
}
