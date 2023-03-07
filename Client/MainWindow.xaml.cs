using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Server;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        core.Client client;
        public ClientWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            client = new core.Client();
            client.AddMessageHandler(MessageShow);
            Task clientTask = new Task(client.Recieve);
            clientTask.Start();
        }
        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.SendAcccount(LoginTextBox.Text);
                client.SetLoginAndPassword(LoginTextBox.Text, PasswordTextBox.Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MessageShow(string message)
        {
            MessageBox.Show(message);
        }
    }
}
