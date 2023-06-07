using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CoreLib;
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
            client.AddChatHandler(PrintChatMess);
        }
        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.SendAcccount(LoginTextBox.Text);
                client.SetLoginAndPassword(LoginTextBox.Text, PasswordTextBox.Password);
                OutputMessageTextBlock.IsEnabled = true;
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

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!client.ChatIsActive)
                return;
            if (string.IsNullOrWhiteSpace(OutputMessageTextBlock.Text))
                return;
            client.ChatSend(OutputMessageTextBlock.Text);
        }

        private void PrintChatMess(string message)
        {
            this.Dispatcher.Invoke(new Action(
            delegate ()
            {
                ShowMessagesTextBlock.Text += message;
            }
            ));

        }
    }
}
