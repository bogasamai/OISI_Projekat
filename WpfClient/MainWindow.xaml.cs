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

            // Start timer to update date and time in status bar
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => {
                StatusDateTime.Text = DateTime.Now.ToString("HH:mm dd.MM.yyyy.");
            };
            timer.Start();

            // Handle tab selection changes to update status bar
            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;

            DataContext = this;
        }

        private void MenuItem_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sajam knjiga v1.8.6, autori- Vukasin Petrovic i Luka Avramovic", "About", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void MenuItem_OpenPosetioci_Click(object sender, RoutedEventArgs e)
        {
            
            MainTabControl.SelectedIndex = 0;
        }

        private void MenuItem_OpenAutori_Click(object sender, RoutedEventArgs e)
        {
           
            MainTabControl.SelectedIndex = 1;
        }

        private void MenuItem_OpenKnjige_Click(object sender, RoutedEventArgs e)
        {
            
            MainTabControl.SelectedIndex = 2;
        }
        private void MenuItem_OpenIzdavaci_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("Otvaranje prozora za upravljanje izdavačima...");
        }

        private void MenuItem_New_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Otvaranje prozora za novi unos...");
        }

        private void MainTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Proveravamo da li je izvor dogadjaja bas TabControl (da ne uhvati klik u tabeli)
            if (e.Source is System.Windows.Controls.TabControl tabControl)
            {
                var selectedTab = tabControl.SelectedItem as System.Windows.Controls.TabItem;
                if (selectedTab != null)
                {
                    // Postavlja format: Sajam knjiga - [Ime Taba] prema Slici 4
                    StatusText.Text = $"Sajam knjiga - {selectedTab.Header}";
                }
            }
        }

        private void MenuItem_Save_Click(object sender, EventArgs e)
        {
            DataHandler.SacuvajPosetioce(Posetioci);
            //DataHandler.SacuvajIzdavace(sviIzdavaci);
            DataHandler.SacuvajAutore(Autori);
            DataHandler.SacuvajKnjige(Knjige);

            MessageBox.Show("Svi novi podaci su uspešno sačuvani u folder 'podaci'.");
            
        }

    }
}