using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
=======
using Server;
using MD5Hash;
using System.Security.Cryptography;
using System.Windows.Controls;
>>>>>>> parent of 21aafab (Revert "Added MD5 to passwords")

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
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();
            DataContext = db.Accounts.Local.ToObservableCollection();
            localAccounts = db.Accounts.ToList();
            DbOutputTextBox.Text = PrintAccounts(localAccounts);
        }
<<<<<<< HEAD

        private void EnterButton_Click(object sender, RoutedEventArgs e)
=======
        private void RegButton_Click(object sender, RoutedEventArgs e)
>>>>>>> parent of 21aafab (Revert "Added MD5 to passwords")
        {
            try
            {
                AddAccount();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Поле {ex.Message} заполнено неверно");
            }
        }
        private void AddAccount()
        {
            Account account = RegisterNewAccount();
<<<<<<< HEAD
            db.Accounts.Add(account);
            db.SaveChanges();
=======
    
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
>>>>>>> parent of 21aafab (Revert "Added MD5 to passwords")

            Log.Text += $"id: {account.id}, login: {account.Login}";
        }
        public Account RegisterNewAccount()
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;
            var passwordHash = MD5Hash.Hash.GetMD5(password);
            int id = localAccounts.Count + 1;
            var account = new Account(id, login, passwordHash);
            return account;
        }
<<<<<<< HEAD
=======
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
>>>>>>> parent of 21aafab (Revert "Added MD5 to passwords")
        public string PrintAccounts(List<Account> accounts)
        {
            if(accounts is null){
                return string.Empty;
            }

            string resultString = string.Empty;
            foreach(var account in accounts)
            {
                resultString += $"id: {account.id}, login: {account.Login}\n";
            }
            return resultString;
        }
    }
}
