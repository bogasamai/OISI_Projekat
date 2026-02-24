using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class IzmenaAutoraWindow : Window
    {
        public Autor IzmenjeniAutor { get; private set; }
        private Autor _originalAutor;
        private List<Knjiga> _autorKnjige;
        private List<Knjiga> _dostupneKnjige; // All available books in the system

        public IzmenaAutoraWindow(Autor a)
        {
            InitializeComponent();
            _originalAutor = a;
            _autorKnjige = new List<Knjiga>(a.SpisakKnjiga ?? new List<Knjiga>());

            // Load all available books from the system (you might need to pass this as parameter)
            _dostupneKnjige = new List<Knjiga>(); // This should be populated with all books from MainWindow

            PopuniPolja(a);
            PopuniKnjigeAutora();
        }

        // Constructor overload that accepts available books
        public IzmenaAutoraWindow(Autor a, List<Knjiga> dostupneKnjige) : this(a)
        {
            _dostupneKnjige = dostupneKnjige ?? new List<Knjiga>();
        }

        private void PopuniPolja(Autor a)
        {
            txtBrojLicne.Text = a.BrojLicneKarte;
            txtIme.Text = a.Ime;
            txtPrezime.Text = a.Prezime;
            dpDatumRodjenja.SelectedDate = a.DatumRodjenja;
            txtTelefon.Text = a.Telefon;
            txtEmail.Text = a.Email;
            txtGodine.Text = a.GodineIskustva.ToString();
            txtAdresa.Text = a.AdresaStanovanja?.Ulica ?? string.Empty;
        }

        private void PopuniKnjigeAutora()
        {
            dgKnjigeAutora.ItemsSource = null;
            dgKnjigeAutora.ItemsSource = _autorKnjige;
        }

        private void DgKnjigeAutora_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnUkloniKnjigu.IsEnabled = dgKnjigeAutora.SelectedItem != null;
        }

        private void BtnDodajKnjigu_Click(object sender, RoutedEventArgs e)
        {
            // Show dialog to select from available books that author hasn't written yet
            var knjigeBezAutora = _dostupneKnjige
                .Where(k => !_autorKnjige.Any(ak => ak.ISBN == k.ISBN))
                .ToList();

            if (!knjigeBezAutora.Any())
            {
                MessageBox.Show("Nema dostupnih knjiga za dodavanje ovom autoru.", "Informacija",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Create and show book selection dialog
            var dialog = new OdaberiKnjiguDialog(knjigeBezAutora);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true && dialog.OdabranaKnjiga != null)
            {
                // Add book to author's list
                _autorKnjige.Add(dialog.OdabranaKnjiga);

                // Also add author to book's author list (bidirectional relationship)
                var selectedBook = dialog.OdabranaKnjiga;
                if (selectedBook.Autori == null)
                    selectedBook.Autori = new List<Autor>();

                // Check if author is not already in the book's author list
                if (!selectedBook.Autori.Any(a => a.BrojLicneKarte == _originalAutor.BrojLicneKarte))
                {
                    selectedBook.Autori.Add(_originalAutor);
                }

                // Refresh the UI
                PopuniKnjigeAutora();

                // Show success message
                MessageBox.Show($"Knjiga '{selectedBook.Naziv}' je uspešno dodana autoru.", "Uspeh",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnUkloniKnjigu_Click(object sender, RoutedEventArgs e)
        {
            if (dgKnjigeAutora.SelectedItem is Knjiga selectedKnjiga)
            {
                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da uklonite knjigu '{selectedKnjiga.Naziv}' od ovog autora?",
                    "Potvrda uklanjanja",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove from author's book list
                    _autorKnjige.Remove(selectedKnjiga);

                    // Also remove author from book's author list (bidirectional relationship)
                    if (selectedKnjiga.Autori != null)
                    {
                        var authorToRemove = selectedKnjiga.Autori
                            .FirstOrDefault(a => a.BrojLicneKarte == _originalAutor.BrojLicneKarte);
                        if (authorToRemove != null)
                        {
                            selectedKnjiga.Autori.Remove(authorToRemove);
                        }
                    }

                    // Refresh the UI
                    PopuniKnjigeAutora();
                    btnUkloniKnjigu.IsEnabled = false;

                    // Show success message
                    MessageBox.Show($"Knjiga '{selectedKnjiga.Naziv}' je uspešno uklonjena od autora.", "Uspeh",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ValidateForm(object sender, EventArgs e)
        {
            if (btnPotvrdi == null) return;

            bool isOk = !string.IsNullOrWhiteSpace(txtBrojLicne.Text) &&
                        !string.IsNullOrWhiteSpace(txtIme.Text) &&
                        !string.IsNullOrWhiteSpace(txtPrezime.Text) &&
                        txtEmail.Text.Contains("@") &&
                        int.TryParse(txtGodine.Text, out _);

            btnPotvrdi.IsEnabled = isOk;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            IzmenjeniAutor = new Autor
            {
                BrojLicneKarte = txtBrojLicne.Text.Trim(),
                Ime = txtIme.Text.Trim(),
                Prezime = txtPrezime.Text.Trim(),
                DatumRodjenja = dpDatumRodjenja.SelectedDate ?? DateTime.Now,
                Telefon = txtTelefon.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                GodineIskustva = int.TryParse(txtGodine.Text.Trim(), out var g) ? g : 0,
                AdresaStanovanja = new Adresa { Ulica = txtAdresa.Text.Trim() },
                SpisakKnjiga = _autorKnjige // Include modified book list
            };

            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}

