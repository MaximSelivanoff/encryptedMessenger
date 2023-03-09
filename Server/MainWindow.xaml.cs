using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext db = new ApplicationContext();
        private CollectionViewSource AccountViewSource;
        core.Server server;
        public MainWindow()
        {
            InitializeComponent();
            AccountViewSource =
               (CollectionViewSource)FindResource(nameof(AccountViewSource));
            Loaded += MainWindowLoaded;
        }
        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();
            db.Accounts.Load();
            DataContext = db.Accounts.Local.ToObservableCollection();
            AccountViewSource.Source = DataContext;
            server = new core.Server();
            server.AddLogHandler(PrintLog);
            Task serverTask = new Task(server.Start);
            serverTask.Start();

            LogTextBox.Text = "Лог сетевого взаимодействия:";
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                server.tryRegistrateAccount(LoginTextBox.Text, PasswordTextBox.Password);
                MessageBox.Show($"Пользователь {LoginTextBox.Text} зарегестрирован");
                AccountsDataGridUpdate();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AccountsDataGridUpdate()
        {
            AccountViewSource.Source = null;
            AccountViewSource.Source = db.Accounts.ToList();
        }
        private void PrintLog(string log)
        {
            this.Dispatcher.Invoke(new Action(
            delegate ()
            {
                LogTextBox.Text += log + "\n\n";
            }
            ));
            
        }
    }
}
