using Core.Data;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
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

        // Pagination / filtering / sorting state
        private const int PageSize = 16;
        private int posetiociPage = 1, autoriPage = 1, knjigePage = 1;
        private List<Posetilac> filteredPosetioci = new List<Posetilac>();
        private List<Autor> filteredAutori = new List<Autor>();
        private List<Knjiga> filteredKnjige = new List<Knjiga>();

        private string posetiociSortMember = null;
        private ListSortDirection posetiociSortDir = ListSortDirection.Ascending;
        private string autoriSortMember = null;
        private ListSortDirection autoriSortDir = ListSortDirection.Ascending;
        private string knjigeSortMember = null;
        private ListSortDirection knjigeSortDir = ListSortDirection.Ascending;

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
            var (autori, knjige, posetioci) = DataHandler.UcitajSve();
            originalAutori = autori;
            originalKnjige = knjige;
            originalPosetioci = posetioci;

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
            // Initialize filtered lists and reset pagination/sorting
            filteredPosetioci = originalPosetioci.ToList();
            filteredAutori = originalAutori.ToList();
            filteredKnjige = originalKnjige.ToList();

            posetiociPage = autoriPage = knjigePage = 1;
            posetiociSortMember = autoriSortMember = knjigeSortMember = null;

            RefreshPosetiociView();
            RefreshAutoriView();
            RefreshKnjigeView();
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

        // --- Refresh / pagination / sorting helpers ---
        private void RefreshPosetiociView()
        {
            if (filteredPosetioci == null) filteredPosetioci = originalPosetioci.ToList();

            IEnumerable<Posetilac> src = filteredPosetioci;
            if (!string.IsNullOrEmpty(posetiociSortMember))
            {
                switch (posetiociSortMember)
                {
                    case "BrojClanskeKarte":
                        src = posetiociSortDir == ListSortDirection.Ascending ? src.OrderBy(p => p.BrojClanskeKarte) : src.OrderByDescending(p => p.BrojClanskeKarte);
                        break;
                    case "Ime":
                        src = posetiociSortDir == ListSortDirection.Ascending ? src.OrderBy(p => p.Ime) : src.OrderByDescending(p => p.Ime);
                        break;
                    case "Prezime":
                        src = posetiociSortDir == ListSortDirection.Ascending ? src.OrderBy(p => p.Prezime) : src.OrderByDescending(p => p.Prezime);
                        break;
                    case "Status":
                        src = posetiociSortDir == ListSortDirection.Ascending ? src.OrderBy(p => p.Status) : src.OrderByDescending(p => p.Status);
                        break;
                }
            }

            int total = src.Count();
            int totalPages = Math.Max(1, (total + PageSize - 1) / PageSize);
            if (posetiociPage > totalPages) posetiociPage = totalPages;

            var pageItems = src.Skip((posetiociPage - 1) * PageSize).Take(PageSize).ToList();

            Posetioci.Clear();
            foreach (var p in pageItems) Posetioci.Add(p);

            if (PosetiociPageText != null) PosetiociPageText.Text = $"{posetiociPage}/{totalPages}";
            // enable/disable pager buttons
            if (BtnPosetiociPrev != null) BtnPosetiociPrev.IsEnabled = posetiociPage > 1;
            if (BtnPosetiociNext != null) BtnPosetiociNext.IsEnabled = posetiociPage < totalPages;
        }

        private void RefreshAutoriView()
        {
            if (filteredAutori == null) filteredAutori = originalAutori.ToList();
            IEnumerable<Autor> src = filteredAutori;
            if (!string.IsNullOrEmpty(autoriSortMember))
            {
                switch (autoriSortMember)
                {
                    case "Ime":
                        src = autoriSortDir == ListSortDirection.Ascending ? src.OrderBy(a => a.Ime) : src.OrderByDescending(a => a.Ime);
                        break;
                    case "Prezime":
                        src = autoriSortDir == ListSortDirection.Ascending ? src.OrderBy(a => a.Prezime) : src.OrderByDescending(a => a.Prezime);
                        break;
                    case "Email":
                        src = autoriSortDir == ListSortDirection.Ascending ? src.OrderBy(a => a.Email) : src.OrderByDescending(a => a.Email);
                        break;
                    case "BrojLicneKarte":
                        src = autoriSortDir == ListSortDirection.Ascending ? src.OrderBy(a => a.BrojLicneKarte) : src.OrderByDescending(a => a.BrojLicneKarte);
                        break;
                    case "DatumRodjenja":
                        src = autoriSortDir == ListSortDirection.Ascending ? src.OrderBy(a => a.DatumRodjenja) : src.OrderByDescending(a => a.DatumRodjenja);
                        break;
                    case "Adresa":
                        src = autoriSortDir == ListSortDirection.Ascending ? src.OrderBy(a => a.AdresaStanovanja?.Ulica) : src.OrderByDescending(a => a.AdresaStanovanja?.Ulica);
                        break;
                }
            }

            int total = src.Count();
            int totalPages = Math.Max(1, (total + PageSize - 1) / PageSize);
            if (autoriPage > totalPages) autoriPage = totalPages;

            var pageItems = src.Skip((autoriPage - 1) * PageSize).Take(PageSize).ToList();

            Autori.Clear();
            foreach (var a in pageItems) Autori.Add(a);

            if (AutoriPageText != null) AutoriPageText.Text = $"{autoriPage}/{totalPages}";
            // enable/disable pager buttons
            if (BtnAutoriPrev != null) BtnAutoriPrev.IsEnabled = autoriPage > 1;
            if (BtnAutoriNext != null) BtnAutoriNext.IsEnabled = autoriPage < totalPages;
        }

        private void RefreshKnjigeView()
        {
            if (filteredKnjige == null) filteredKnjige = originalKnjige.ToList();
            IEnumerable<Knjiga> src = filteredKnjige;
            if (!string.IsNullOrEmpty(knjigeSortMember))
            {
                switch (knjigeSortMember)
                {
                    case "ISBN":
                        src = knjigeSortDir == ListSortDirection.Ascending ? src.OrderBy(k => k.ISBN) : src.OrderByDescending(k => k.ISBN);
                        break;
                    case "Naziv":
                        src = knjigeSortDir == ListSortDirection.Ascending ? src.OrderBy(k => k.Naziv) : src.OrderByDescending(k => k.Naziv);
                        break;
                    case "Cena":
                        src = knjigeSortDir == ListSortDirection.Ascending ? src.OrderBy(k => k.Cena) : src.OrderByDescending(k => k.Cena);
                        break;
                    case "GodinaIzdanja":
                        src = knjigeSortDir == ListSortDirection.Ascending ? src.OrderBy(k => k.GodinaIzdanja) : src.OrderByDescending(k => k.GodinaIzdanja);
                        break;
                    case "Zanr":
                        src = knjigeSortDir == ListSortDirection.Ascending ? src.OrderBy(k => k.Zanr) : src.OrderByDescending(k => k.Zanr);
                        break;
                }
            }

            int total = src.Count();
            int totalPages = Math.Max(1, (total + PageSize - 1) / PageSize);
            if (knjigePage > totalPages) knjigePage = totalPages;

            var pageItems = src.Skip((knjigePage - 1) * PageSize).Take(PageSize).ToList();

            Knjige.Clear();
            foreach (var k in pageItems) Knjige.Add(k);

            if (KnjigePageText != null) KnjigePageText.Text = $"{knjigePage}/{totalPages}";
            // enable/disable pager buttons
            if (BtnKnjigePrev != null) BtnKnjigePrev.IsEnabled = knjigePage > 1;
            if (BtnKnjigeNext != null) BtnKnjigeNext.IsEnabled = knjigePage < totalPages;
        }

        // Sorting handlers
        private void PosetiociGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            string member = e.Column.SortMemberPath;
            if (string.IsNullOrEmpty(member)) return;
            if (posetiociSortMember == member)
                posetiociSortDir = posetiociSortDir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else
            {
                posetiociSortMember = member;
                posetiociSortDir = ListSortDirection.Ascending;
            }
            // update column sort indicators
            foreach (var col in PosetiociGrid.Columns) col.SortDirection = null;
            e.Column.SortDirection = posetiociSortDir;
            posetiociPage = 1;
            RefreshPosetiociView();
        }

        private void AutoriGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            string member = e.Column.SortMemberPath;
            if (string.IsNullOrEmpty(member)) return;
            if (autoriSortMember == member)
                autoriSortDir = autoriSortDir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else
            {
                autoriSortMember = member;
                autoriSortDir = ListSortDirection.Ascending;
            }
            foreach (var col in AutoriGrid.Columns) col.SortDirection = null;
            e.Column.SortDirection = autoriSortDir;
            autoriPage = 1;
            RefreshAutoriView();
        }

        private void KnjigeGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            string member = e.Column.SortMemberPath;
            if (string.IsNullOrEmpty(member)) return;
            if (knjigeSortMember == member)
                knjigeSortDir = knjigeSortDir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else
            {
                knjigeSortMember = member;
                knjigeSortDir = ListSortDirection.Ascending;
            }
            foreach (var col in KnjigeGrid.Columns) col.SortDirection = null;
            e.Column.SortDirection = knjigeSortDir;
            knjigePage = 1;
            RefreshKnjigeView();
        }

        // Pager handlers
        private void BtnPosetiociPrev_Click(object sender, RoutedEventArgs e) { if (posetiociPage > 1) { posetiociPage--; RefreshPosetiociView(); } }
        private void BtnPosetiociNext_Click(object sender, RoutedEventArgs e) { posetiociPage++; RefreshPosetiociView(); }

        private void BtnAutoriPrev_Click(object sender, RoutedEventArgs e) { if (autoriPage > 1) { autoriPage--; RefreshAutoriView(); } }
        private void BtnAutoriNext_Click(object sender, RoutedEventArgs e) { autoriPage++; RefreshAutoriView(); }

        private void BtnKnjigePrev_Click(object sender, RoutedEventArgs e) { if (knjigePage > 1) { knjigePage--; RefreshKnjigeView(); } }
        private void BtnKnjigeNext_Click(object sender, RoutedEventArgs e) { knjigePage++; RefreshKnjigeView(); }

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

            // Set filtered list and refresh with pagination/sorting
            filteredPosetioci = foundPosetioci;
            posetiociPage = 1;
            RefreshPosetiociView();
            StatusText.Text = $"Pronađeno {filteredPosetioci.Count} posetilaca";
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

            filteredAutori = foundAutori;
            autoriPage = 1;
            RefreshAutoriView();

            StatusText.Text = $"Pronađeno {filteredAutori.Count} autora";
        }

        private void SearchKnjige(string searchInput)
        {
            // Pretraga knjiga se obavlja unosom dela naziva knjige ili dela ISBN broja
            string searchTerm = searchInput.ToLower().Trim();

            var foundKnjige = originalKnjige
                .Where(k => k.NazivDisplay.ToLower().Contains(searchTerm) ||
                           k.ISBNDisplay.ToLower().Contains(searchTerm))
                .ToList();
            filteredKnjige = foundKnjige;
            knjigePage = 1;
            RefreshKnjigeView();

            StatusText.Text = $"Pronađeno {filteredKnjige.Count} knjiga";
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

            if (header.Contains("autor") || header.Contains("autori"))
            {
                OpenAddAutorDialog();
            }
            else if (header.Contains("poset") || header.Contains("posetioci"))
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
                    var dlg = new IzmenaAutoraWindow(selektovan, originalKnjige);
                    dlg.Owner = this;
                    if (dlg.ShowDialog() == true)
                    {
                        int index = originalAutori.IndexOf(selektovan);
                        if (index != -1)
                        {
                            originalAutori[index] = dlg.IzmenjeniAutor;

                            // Sinhronizuj veze u originalKnjige
                            var noviAutor = dlg.IzmenjeniAutor;
                            var stareKnjige = selektovan.SpisakKnjiga ?? new List<Knjiga>();
                            var noveKnjige = noviAutor.SpisakKnjiga ?? new List<Knjiga>();
                            foreach (var knjiga in noveKnjige)
                            {
                                var origKnjiga = originalKnjige.FirstOrDefault(k => k.ISBN == knjiga.ISBN);
                                if (origKnjiga != null)
                                {
                                    if (origKnjiga.Autori == null) origKnjiga.Autori = new List<Autor>();
                                    if (!origKnjiga.Autori.Any(a => a.BrojLicneKarte == noviAutor.BrojLicneKarte))
                                    {
                                        origKnjiga.Autori.Add(noviAutor);
                                    }
                                    else
                                    {
                                        var stari = origKnjiga.Autori.FirstOrDefault(a => a.BrojLicneKarte == noviAutor.BrojLicneKarte);
                                        if (stari != null)
                                        {
                                            int idx = origKnjiga.Autori.IndexOf(stari);
                                            origKnjiga.Autori[idx] = noviAutor;
                                        }
                                    }
                                }
                            }

                            // Ukloni autora sa knjiga koje više nisu u spisku
                            foreach (var staraKnjiga in stareKnjige)
                            {
                                if (!noveKnjige.Any(nk => nk.ISBN == staraKnjiga.ISBN))
                                {
                                    var origKnjiga = originalKnjige.FirstOrDefault(k => k.ISBN == staraKnjiga.ISBN);
                                    if (origKnjiga != null && origKnjiga.Autori != null)
                                    {
                                        var zaUklanjanje = origKnjiga.Autori.FirstOrDefault(a => a.BrojLicneKarte == noviAutor.BrojLicneKarte);
                                        if (zaUklanjanje != null)
                                            origKnjiga.Autori.Remove(zaUklanjanje);
                                    }
                                }
                            }

                            // Ažuriraj SpisakKnjiga za novog autora na osnovu aktuelnog stanja originalKnjige
                            noviAutor.SpisakKnjiga = originalKnjige
                                .Where(k => k.Autori != null && k.Autori.Any(a => a.BrojLicneKarte == noviAutor.BrojLicneKarte))
                                .ToList();

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
                        // Ukloni autora sa svih knjiga
                        foreach (var knjiga in originalKnjige)
                        {
                            if (knjiga.Autori != null)
                            {
                                var zaUklanjanje = knjiga.Autori.FirstOrDefault(au => au.BrojLicneKarte == a.BrojLicneKarte);
                                if (zaUklanjanje != null)
                                    knjiga.Autori.Remove(zaUklanjanje);
                            }
                        }
                    }
                }
                else if (active == 2)
                {
                    var k = KnjigeGrid.SelectedItem as Knjiga;
                    if (k != null)
                    {
                        originalKnjige.Remove(k);
                        Knjige.Remove(k);
                        // Ukloni knjigu iz spiska svih autora
                        foreach (var autor in originalAutori)
                        {
                            if (autor.SpisakKnjiga != null)
                            {
                                var zaUklanjanje = autor.SpisakKnjiga.FirstOrDefault(kn => kn.ISBN == k.ISBN);
                                if (zaUklanjanje != null)
                                    autor.SpisakKnjiga.Remove(zaUklanjanje);
                            }
                        }
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

        // Header arrow click handler (Tag format: "Grid:Member:Asc|Desc")
        private void HeaderSort_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                var parts = tag.Split(':');
                if (parts.Length != 3) return;
                var grid = parts[0];
                var member = parts[1];
                var dir = parts[2] == "Asc" ? ListSortDirection.Ascending : ListSortDirection.Descending;

                switch (grid)
                {
                    case "Posetioci":
                        posetiociSortMember = member;
                        posetiociSortDir = dir;
                        // update column indicators
                        foreach (var col in PosetiociGrid.Columns) col.SortDirection = null;
                        var c1 = PosetiociGrid.Columns.FirstOrDefault(c => c.SortMemberPath == member);
                        if (c1 != null) c1.SortDirection = dir;
                        posetiociPage = 1;
                        RefreshPosetiociView();
                        break;
                    case "Autori":
                        autoriSortMember = member;
                        autoriSortDir = dir;
                        foreach (var col in AutoriGrid.Columns) col.SortDirection = null;
                        var c2 = AutoriGrid.Columns.FirstOrDefault(c => c.SortMemberPath == member);
                        if (c2 != null) c2.SortDirection = dir;
                        autoriPage = 1;
                        RefreshAutoriView();
                        break;
                    case "Knjige":
                        knjigeSortMember = member;
                        knjigeSortDir = dir;
                        foreach (var col in KnjigeGrid.Columns) col.SortDirection = null;
                        var c3 = KnjigeGrid.Columns.FirstOrDefault(c => c.SortMemberPath == member);
                        if (c3 != null) c3.SortDirection = dir;
                        knjigePage = 1;
                        RefreshKnjigeView();
                        break;
                }
            }
        }
    }
}