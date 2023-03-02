using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Server;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext db = new ApplicationContext();
        List<Account> accounts;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();
            db.Accounts.Load();
            DataContext = db.Accounts.Local.ToObservableCollection();

            accounts = db.Accounts.ToList();
            DbOutputTextBox.Text = PrintAccounts(accounts);
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddAccount();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void AddAccount()
        {
            Account account = RegisterNewAccount();
            db.Accounts.Add(account);
            db.SaveChanges();
        }

        private Account RegisterNewAccount()
        {
            var account = CreateNewAccount();
            if (IsUnicLogin(account))
                return account;
            else throw new ArgumentException("Этот логин уже занят");
        }
        private Account CreateNewAccount()
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;
            int id = accounts.Count + 1;
            var account = new Account(id, login, password);
            return account;
        }
        private bool IsUnicLogin(Account account)
        {
            foreach (Account a in accounts)
                if (account.Login == a.Login)
                    return false;
            return true;
        }
        private bool IsCorrectPassword(Account account)
        {
           var result = db.Accounts.Select(a => a).
                Where(a => a.Login.
                Equals(account.Login)).
                Where(a => a.Password.
                Equals(account.Password));
            if(result.ToList().Count == 0)
                return false;
            else 
                return true;
        }
        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                var account = CreateNewAccount();
                if (!IsUnicLogin(account))
                {
                    if (IsCorrectPassword(account))
                    {
                        MessageBox.Show("Вы вошли в аккаунт");
                    }
                    else
                        throw new ArgumentException("Неверный пароль");
                }
                else
                {
                    throw new ArgumentException("Неверный логин");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public string PrintAccounts(List<Account> accounts)
        {
            string resultString = "";
            foreach(Account account in accounts)
            {
                resultString += $"id: {account.Id}, login: {account.Login}\n";
            }
            return resultString;
        }


    }
}
