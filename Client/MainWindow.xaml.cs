using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Server;
using MD5Hash;
using System.Security.Cryptography;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext db = new ApplicationContext();
        List<Account> localAccounts;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();
            DataContext = db.Accounts.Local.ToObservableCollection();
            localAccounts = db.Accounts.ToList();
            DbOutputTextBox.Text = PrintAccounts(localAccounts);
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
    
            try
            {
                db.Accounts.Add(account);
                db.SaveChanges();
            }
            catch (Exception ex )
            {
                Console.WriteLine(ex.Message);
                return;
            }

            localAccounts.Add(account);
            DbOutputTextBox.Text = PrintAccounts(localAccounts);
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
            var passwordHash = MD5Hash.Hash.GetMD5(password);
            int id = localAccounts.Count + 1;
            var account = new Account(id, login, passwordHash);
            return account;
        }
        private bool IsUnicLogin(Account account)
        {
            foreach (Account a in localAccounts)
                if (account.Login == a.Login)
                    return false;
            return true;
        }
        private bool IsCorrectPassword(Account account)
        {
           var result = db.Accounts
                .Where(a => a.Login
                .Equals(account.Login))
                .Where(a => a.Password
                .Equals(account.Password));

            if(result.ToList().Count == 0)
                return false;

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
                        MessageBox.Show("Вы вошли в аккаунт");
                    else
                        throw new ArgumentException("Неверный пароль");
                }
                else
                    throw new ArgumentException("Неверный логин");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public string PrintAccounts(List<Account> accounts)
        {
            if(accounts is null){
                return string.Empty;
            }

            string resultString = string.Empty;
            foreach(var account in accounts)
            {
                resultString += $"id: {account.Id}, login: {account.Login}\n";
            }
            return resultString;
        }
    }
}
