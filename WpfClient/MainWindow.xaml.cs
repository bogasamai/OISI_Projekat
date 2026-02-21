using Core.Data;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfClient
{
    public partial class MainWindow : Window
    {
        public List<Posetilac> Posetioci { get; set; } = new List<Posetilac>();
        public List<Autor> Autori { get; set; } = new List<Autor>();
        public List<Knjiga> Knjige { get; set; } = new List<Knjiga>();

        public MainWindow()
        {
            InitializeComponent();

            // Set initial size to 3/4 of screen and center
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            Width = screenWidth * 3.0 / 4.0;
            Height = screenHeight * 3.0 / 4.0;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Load data
            Posetioci = DataHandler.UcitajPosetioce();
            Autori = DataHandler.UcitajAutore();
            Knjige = DataHandler.UcitajKnjige();

            DataContext = this;
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sajam knjiga - WPF klijent", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Toolbar_Add_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Add clicked";
        }

        private void Toolbar_Edit_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Edit clicked";
        }

        private void Toolbar_Delete_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Delete clicked";
        }
    }
}