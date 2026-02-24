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
            string searchInput = SearchTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchInput))
            {
                ResetCollectionsToOriginal();
                StatusText.Text = "Sajam knjiga - prikazani su svi entiteti";
            }
            else
            {
                int activeTabIndex = MainTabControl.SelectedIndex;
                switch (activeTabIndex)
                {
                    case 0: // Posetioci
                        SearchPosetioci(searchInput);
                        break;
                    case 1: // Autori
                        SearchAutori(searchInput);
                        break;
                    case 2: // Knjige
                        SearchKnjige(searchInput);
                        break;
                }
            }
        }

        private void SearchPosetioci(string searchInput)
        {
            // Parse comma-separated words (case-insensitive)
            var words = searchInput.Split(',')
                                   .Select(w => w.Trim().ToLower())
                                   .Where(w => !string.IsNullOrEmpty(w))
                                   .ToArray();

            List<Posetilac> foundPosetioci = new List<Posetilac>();

            if (words.Length == 1)
            {
                // Jedna reč → prikazuju se posetioci čije prezime sadrži unetu reč
                string prezimeSearch = words[0];
                foundPosetioci = originalPosetioci
                    .Where(p => p.PrezimeDisplay.ToLower().Contains(prezimeSearch))
                    .ToList();
            }
            else if (words.Length == 2)
            {
                // Dve reči → prva reč mora biti sadržana u prezimenu, a druga u imenu
                string prezimeSearch = words[0];
                string imeSearch = words[1];
                foundPosetioci = originalPosetioci
                    .Where(p => p.PrezimeDisplay.ToLower().Contains(prezimeSearch) &&
                               p.ImeDisplay.ToLower().Contains(imeSearch))
                    .ToList();
            }
            else if (words.Length == 3)
            {
                // Tri reči → prva reč mora biti deo broja članske kartice, druga deo imena, a treća deo prezimena
                string brojKarticeSearch = words[0];
                string imeSearch = words[1];
                string prezimeSearch = words[2];
                foundPosetioci = originalPosetioci
                    .Where(p => p.BrojClanskeKarteDisplay.ToLower().Contains(brojKarticeSearch) &&
                               p.ImeDisplay.ToLower().Contains(imeSearch) &&
                               p.PrezimeDisplay.ToLower().Contains(prezimeSearch))
                    .ToList();
            }
            else
            {
                // Više od 3 reči - ne pretražuj
                foundPosetioci = new List<Posetilac>();
            }

            Posetioci.Clear();
            foreach (var p in foundPosetioci)
                Posetioci.Add(p);

            StatusText.Text = $"Pronađeno {Posetioci.Count} posetilaca";
        }

        private void SearchAutori(string searchInput)
        {
            // Parse comma-separated words (case-insensitive)
            var words = searchInput.Split(',')
                                   .Select(w => w.Trim().ToLower())
                                   .Where(w => !string.IsNullOrEmpty(w))
                                   .ToArray();

            List<Autor> foundAutori = new List<Autor>();

            if (words.Length == 1)
            {
                // Jedna reč → prikazuju se autori čije prezime sadrži unetu reč
                string prezimeSearch = words[0];
                foundAutori = originalAutori
                    .Where(a => a.PrezimeDisplay.ToLower().Contains(prezimeSearch))
                    .ToList();
            }
            else if (words.Length == 2)
            {
                // Dve reči → prva reč mora biti sadržana u prezimenu, a druga u imenu
                string prezimeSearch = words[0];
                string imeSearch = words[1];
                foundAutori = originalAutori
                    .Where(a => a.PrezimeDisplay.ToLower().Contains(prezimeSearch) &&
                               a.ImeDisplay.ToLower().Contains(imeSearch))
                    .ToList();
            }
            else
            {
                // Više od 2 reči - ne pretražuj
                foundAutori = new List<Autor>();
            }

            Autori.Clear();
            foreach (var a in foundAutori)
                Autori.Add(a);

            StatusText.Text = $"Pronađeno {Autori.Count} autora";
        }

        private void SearchKnjige(string searchInput)
        {
            // Pretraga knjiga se obavlja unosom dela naziva knjige ili dela ISBN broja
            string searchTerm = searchInput.ToLower().Trim();

            var foundKnjige = originalKnjige
                .Where(k => k.NazivDisplay.ToLower().Contains(searchTerm) ||
                           k.ISBNDisplay.ToLower().Contains(searchTerm))
                .ToList();

            Knjige.Clear();
            foreach (var k in foundKnjige)
                Knjige.Add(k);

            StatusText.Text = $"Pronađeno {Knjige.Count} knjiga";
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
            OpenAddForActiveTab();
        }

        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            OpenAddForActiveTab();
        }

        private void OpenAddForActiveTab()
        {
            // Determine active tab reliably using selected TabItem header
            if (MainTabControl == null) { OpenAddPosetilacDialog(); return; }
            var selected = MainTabControl.SelectedItem as TabItem;
            string header = selected?.Header?.ToString() ?? string.Empty;
            header = header.Trim().ToLowerInvariant();

            if (header.Contains("autor") || header.Contains("autori") )
            {
                OpenAddAutorDialog();
            }
            else if (header.Contains("poset") || header.Contains("posetioci") )
            {
                OpenAddPosetilacDialog();
            }
            else if (header.Contains("knjig"))
            {
                OpenAddKnjigaDialog();
            }
            else
            {
                // default
                OpenAddPosetilacDialog();
            }
        }

        private void OpenAddPosetilacDialog()
        {
            var dlg = new DodajPosetiocaWindow();
            dlg.Owner = this;
            bool? res = dlg.ShowDialog();
            if (res == true && dlg.NoviPosetilac != null)
            {
                originalPosetioci.Add(dlg.NoviPosetilac);
                Posetioci.Add(dlg.NoviPosetilac);
                StatusText.Text = "Posetilac dodat";
            }
        }
        private void OpenAddKnjigaDialog()
        {
            var dlg = new DodajKnjiguWindow();
            dlg.Owner = this;
            bool? res = dlg.ShowDialog();

            if (res == true && dlg.NovaKnjiga != null)
            {
                // Dodajemo u listu koja se serijalizuje
                originalKnjige.Add(dlg.NovaKnjiga);
                // Dodajemo u ObservableCollection koja je vezana za DataGrid
                Knjige.Add(dlg.NovaKnjiga);

                StatusText.Text = "Knjiga uspešno dodata";
            }
        }


        private void OpenAddAutorDialog()
        {
            var dlg = new AddAutorWindow();
            dlg.Owner = this;
            bool? res = dlg.ShowDialog();
            if (res == true && dlg.Result != null)
            {
                originalAutori.Add(dlg.Result);
                Autori.Add(dlg.Result);
                StatusText.Text = "Autor dodat";
            }
        }

        private void Toolbar_Edit_Click(object sender, RoutedEventArgs e)
        {
            int activeTab = MainTabControl.SelectedIndex;

            // Proveravamo da li je aktivan tab Posetioci (indeks 0)
            if (activeTab == 0)
            {
                var selektovan = PosetiociGrid.SelectedItem as Posetilac;
                if (selektovan != null)
                {
                    var dlg = new IzmenaPosetiocaWindow(selektovan);
                    dlg.Owner = this;
                    if (dlg.ShowDialog() == true)
                    {
                        // Ažuriramo listu (nađi stari, ubaci novi)
                        int index = originalPosetioci.IndexOf(selektovan);
                        if (index != -1)
                        {
                            originalPosetioci[index] = dlg.IzmenjeniPosetilac;
                            ResetCollectionsToOriginal(); // Osveži UI
                            StatusText.Text = "Posetilac uspešno izmenjen.";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Molimo izaberite posetioca iz tabele.");
                }
            }
            // Proveravamo da li je aktivan tab Autori (indeks 1)
            else if (activeTab == 1)
            {
                var selektovan = AutoriGrid.SelectedItem as Autor;
                if (selektovan != null)
                {
                    var dlg = new IzmenaAutoraWindow(selektovan);
                    dlg.Owner = this;
                    if (dlg.ShowDialog() == true)
                    {
                        // Ažuriramo listu (nađi stari, ubaci novi)
                        int index = originalAutori.IndexOf(selektovan);
                        if (index != -1)
                        {
                            originalAutori[index] = dlg.IzmenjeniAutor;
                            ResetCollectionsToOriginal(); // Osveži UI
                            StatusText.Text = "Autor uspešno izmenjen.";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Molimo izaberite autora iz tabele.");
                }
            }
            else if (activeTab == 2)
            {
                var selektovana = KnjigeGrid.SelectedItem as Knjiga;
                if (selektovana != null)
                {
                    var dlg = new IzmenaKnjigeWindow(selektovana);
                    dlg.Owner = this;
                    if (dlg.ShowDialog() == true && dlg.IzmenjenaKnjiga != null)
                    {
                        // Pronađi indeks u originalnoj listi
                        int index = originalKnjige.IndexOf(selektovana);
                        if (index != -1)
                        {
                            // Zameni podatke u originalnoj listi
                            originalKnjige[index] = dlg.IzmenjenaKnjiga;
                            // Osveži ObservableCollection i UI
                            ResetCollectionsToOriginal();
                            StatusText.Text = "Knjiga uspešno izmenjena.";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Molimo izaberite knjigu iz tabele.");
                }
            }

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
            MenuItem_New_Click(sender, new RoutedEventArgs());
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