using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext db = new ApplicationContext();
        private CollectionViewSource AccountViewSource;

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
        }
    }
}
