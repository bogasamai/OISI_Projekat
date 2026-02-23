using Core.Data;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace WpfClient
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Posetilac> Posetioci { get; set; } = new ObservableCollection<Posetilac>();
        public ObservableCollection<Autor> Autori { get; set; } = new ObservableCollection<Autor>();
        public ObservableCollection<Knjiga> Knjige { get; set; } = new ObservableCollection<Knjiga>();

        private List<Posetilac> originalPosetioci = new List<Posetilac>();
        private List<Autor> originalAutori = new List<Autor>();
        private List<Knjiga> originalKnjige = new List<Knjiga>();

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
            originalPosetioci = DataHandler.UcitajPosetioce();
            originalAutori = DataHandler.UcitajAutore();
            originalKnjige = DataHandler.UcitajKnjige();

            // Initialize observable collections from originals
            ResetCollectionsToOriginal();

            // Start timer to update date and time in status bar
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                StatusDateTime.Text = DateTime.Now.ToString("HH:mm dd.MM.yyyy.");
            };
            timer.Start();

            // Handle tab selection changes to update status bar
            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;

            // Handle keyboard shortcuts
            this.KeyDown += MainWindow_KeyDown;

            // Handle search textbox changes for real-time search
            if (SearchTextBox != null)
            {
                SearchTextBox.TextChanged += SearchTextBox_TextChanged;
                // Enter key triggers search
                SearchTextBox.KeyDown += (s, e) => { if (e.Key == Key.Enter) PerformSearch(); };
            }

            DataContext = this;
        }

        private void ResetCollectionsToOriginal()
        {
            Posetioci.Clear();
            foreach (var p in originalPosetioci) Posetioci.Add(p);

            Autori.Clear();
            foreach (var a in originalAutori) Autori.Add(a);

            Knjige.Clear();
            foreach (var k in originalKnjige) Knjige.Add(k);
        }

        private void PerformSearch()
        {
            if (SearchTextBox == null) return;
            string searchTerm = SearchTextBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ResetCollectionsToOriginal();
                StatusText.Text = "Sajam knjiga - prikazani su svi entiteti";
            }
            else
            {
                int activeTabIndex = MainTabControl.SelectedIndex;
                switch (activeTabIndex)
                {
                    case 0:
                        var foundP = originalPosetioci
                            .Where(p => p.ImeDisplay.ToLower().Contains(searchTerm) ||
                                        p.PrezimeDisplay.ToLower().Contains(searchTerm) ||
                                        p.BrojClanskeKarteDisplay.ToLower().Contains(searchTerm) ||
                                        p.AdresaDisplay.ToLower().Contains(searchTerm))
                            .ToList();
                        Posetioci.Clear();
                        foreach (var p in foundP) Posetioci.Add(p);
                        StatusText.Text = $"Pronađeno {Posetioci.Count} posetilaca";
                        break;
                    case 1:
                        var foundA = originalAutori
                            .Where(a => a.ImeDisplay.ToLower().Contains(searchTerm) ||
                                        a.PrezimeDisplay.ToLower().Contains(searchTerm) ||
                                        a.BrojLicneKarteDisplay.ToLower().Contains(searchTerm) ||
                                        a.EmailDisplay.ToLower().Contains(searchTerm))
                            .ToList();
                        Autori.Clear();
                        foreach (var a in foundA) Autori.Add(a);
                        StatusText.Text = $"Pronađeno {Autori.Count} autora";
                        break;
                    case 2:
                        var foundK = originalKnjige
                            .Where(k => k.NazivDisplay.ToLower().Contains(searchTerm) ||
                                        k.ISBNDisplay.ToLower().Contains(searchTerm) ||
                                        k.ZanrDisplay.ToLower().Contains(searchTerm))
                            .ToList();
                        Knjige.Clear();
                        foreach (var k in foundK) Knjige.Add(k);
                        StatusText.Text = $"Pronađeno {Knjige.Count} knjiga";
                        break;
                }
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Optional: real-time search while typing. Comment out if only button-triggered search is desired.
            //PerformSearch();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N: Toolbar_Add_Click(null, null); e.Handled = true; break;
                    case Key.E: Toolbar_Edit_Click(null, null); e.Handled = true; break;
                    case Key.D: Toolbar_Delete_Click(null, null); e.Handled = true; break;
                    case Key.F: if (SearchTextBox != null) SearchTextBox.Focus(); e.Handled = true; break;
                }
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sajam knjiga v1.8.6\nAutori: Vukasin Petrovic i Luka Avramovic", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Toolbar_Add_Click(object sender, RoutedEventArgs e)
        {
            int activeTab = MainTabControl.SelectedIndex;

            if (activeTab == 0) // Tab Posetioci
            {
                DodajPosetiocaWindow prozor = new DodajPosetiocaWindow();
                prozor.Owner = this; // Postavlja MainWindow kao roditelja (centriranje)

                if (prozor.ShowDialog() == true)
                {
                    // Dodaj u originalnu listu i u ObservableCollection da se UI odmah osveži
                    originalPosetioci.Add(prozor.NoviPosetilac);
                    Posetioci.Add(prozor.NoviPosetilac);

                    StatusText.Text = "Novi posetilac uspešno dodat.";
                }
            }
            else
            {
                MessageBox.Show("Dodavanje za ovaj tab će biti implementirano uskoro.");
            }
        }

        private void Toolbar_Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = GetSelectedItemFromActiveTab();
            if (selectedItem == null)
            {
                StatusText.Text = "Molimo izaberite entitet za izmenu";
                MessageBox.Show("Prvo izaberite entitet koji želite da izmenite.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            StatusText.Text = "Izmena entiteta - nije implementirano";
            MessageBox.Show("Izmena entiteta će biti implementirana.", "Info", MessageBoxButton.OK);
        }

        private void Toolbar_Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = GetSelectedItemFromActiveTab();
            if (selectedItem == null)
            {
                StatusText.Text = "Molimo izaberite entitet za brisanje";
                MessageBox.Show("Prvo izaberite entitet koji želite da obrišete.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Da li ste sigurni da želite da obrišete odabrani entitet?", "Potvrda", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Remove from originals and UI collection
                int active = MainTabControl.SelectedIndex;
                if (active == 0)
                {
                    var p = PosetiociGrid.SelectedItem as Posetilac;
                    if (p != null)
                    {
                        originalPosetioci.Remove(p);
                        Posetioci.Remove(p);
                    }
                }
                else if (active == 1)
                {
                    var a = AutoriGrid.SelectedItem as Autor;
                    if (a != null)
                    {
                        originalAutori.Remove(a);
                        Autori.Remove(a);
                    }
                }
                else if (active == 2)
                {
                    var k = KnjigeGrid.SelectedItem as Knjiga;
                    if (k != null)
                    {
                        originalKnjige.Remove(k);
                        Knjige.Remove(k);
                    }
                }

                StatusText.Text = "Entitet obrisan";
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                var selectedTab = tabControl.SelectedItem as TabItem;
                if (selectedTab != null)
                {
                    StatusText.Text = $"Sajam knjiga - {selectedTab.Header}";
                }
            }
        }

        private object GetSelectedItemFromActiveTab()
        {
            int activeTabIndex = MainTabControl.SelectedIndex;
            switch (activeTabIndex)
            {
                case 0: return PosetiociGrid.SelectedItem;
                case 1: return AutoriGrid.SelectedItem;
                case 2: return KnjigeGrid.SelectedItem;
                default: return null;
            }
        }

        // Menu click handlers used by MenuItems in XAML
        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            Toolbar_Add_Click(sender, e);
        }

        private void MenuItem_OpenPosetioci_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 0;
            StatusText.Text = "Sajam knjiga - Posetioci";
        }

        private void MenuItem_OpenAutori_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1;
            StatusText.Text = "Sajam knjiga - Autori";
        }

        private void MenuItem_OpenKnjige_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 2;
            StatusText.Text = "Sajam knjiga - Knjige";
        }

        private void MenuItem_OpenIzdavaci_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Otvaranje prozora za upravljanje izdavačima...", "Izdavači", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Menu Save click handler
        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            DataHandler.SacuvajPosetioce(originalPosetioci);
            DataHandler.SacuvajAutore(originalAutori);
            DataHandler.SacuvajKnjige(originalKnjige);
            MessageBox.Show("Svi novi podaci su uspešno sačuvani u folder 'podaci'.");
        }

        // CommandBinding handlers (for keyboard shortcuts) - delegate to click handlers
        private void MenuCommand_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Toolbar_Add_Click(sender, e);
        }


        private void MenuCommand_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_Save_Click(sender, new RoutedEventArgs());
        }

        private void MenuCommand_Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_Exit_Click(sender, new RoutedEventArgs());
        }
    }
}