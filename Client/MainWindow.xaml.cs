using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;

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
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();
            db.Accounts.Load();
            DataContext = db.Accounts.Local.ToObservableCollection();

            accounts = db.Accounts.ToList();
            DbOutputTextBox.Text = PrintAccounts(accounts);
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
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
            db.Accounts.Add(account);
            db.SaveChanges();

            Log.Text += $"id: {account.id}, login: {account.Login}";
        }
        public Account RegisterNewAccount()
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;
            int id = accounts.Count + 1;
            var account = new Account(id, login, password);
            return account;
        }
        public string PrintAccounts(List<Account> accounts)
        {
            string resultString = "";
            foreach(Account account in accounts)
            {
                resultString += $"id: {account.id}, login: {account.Login}\n";
            }
            return resultString;
        }
    }
}
