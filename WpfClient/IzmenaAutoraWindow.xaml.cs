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
        private List<Knjiga> _dostupneKnjige;

        public IzmenaAutoraWindow(Autor a)
        {
            InitializeComponent();
            _originalAutor = a;
            _autorKnjige = new List<Knjiga>(a.SpisakKnjiga ?? new List<Knjiga>());
            _dostupneKnjige = new List<Knjiga>();

            PopuniPolja(a);
            PopuniKnjigeAutora();
        }

        public IzmenaAutoraWindow(Autor a, List<Knjiga> dostupneKnjige) : this(a)
        {
            _dostupneKnjige = dostupneKnjige ?? new List<Knjiga>();


            _autorKnjige = _dostupneKnjige
                .Where(k => k.Autori != null && k.Autori.Any(au => au.BrojLicneKarte == a.BrojLicneKarte))
                .ToList();

            PopuniKnjigeAutora(); // Ponovo popuni grid sa osvezenom listom
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

            if (a.AdresaStanovanja != null)
            {
                string ulica = a.AdresaStanovanja.Ulica ?? "";
                string broj = a.AdresaStanovanja.Broj ?? "";
                string grad = a.AdresaStanovanja.Grad ?? "";
                string drzava = a.AdresaStanovanja.Drzava ?? "";
                txtAdresa.Text = $"{ulica}, {broj}, {grad}, {drzava}";
            }
            else
            {
                txtAdresa.Text = string.Empty;
            }
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
            var knjigeBezAutora = _dostupneKnjige
                .Where(k => !_autorKnjige.Any(ak => ak.ISBN == k.ISBN))
                .Where(k => k.Autori == null || k.Autori.Count == 0)
                .ToList();

            if (!knjigeBezAutora.Any())
            {
                MessageBox.Show("Nema dostupnih knjiga za dodavanje ovom autoru.", "Informacija",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new OdaberiKnjiguDialog(knjigeBezAutora);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true && dialog.OdabraneKnjige.Any())
            {
                foreach (var selectedBook in dialog.OdabraneKnjige)
                {
                    if (_autorKnjige.Any(k => k.ISBN == selectedBook.ISBN)) continue;

                    _autorKnjige.Add(selectedBook);

                    if (selectedBook.Autori == null)
                        selectedBook.Autori = new List<Autor>();

                    if (!selectedBook.Autori.Any(a => a.BrojLicneKarte == _originalAutor.BrojLicneKarte))
                    {
                        selectedBook.Autori.Add(_originalAutor);
                    }
                }

                PopuniKnjigeAutora();

                MessageBox.Show("Knjige su uspešno dodate autoru.", "Uspeh",
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
                    _autorKnjige.Remove(selectedKnjiga);

                    if (selectedKnjiga.Autori != null)
                    {
                        var authorToRemove = selectedKnjiga.Autori
                            .FirstOrDefault(a => a.BrojLicneKarte == _originalAutor.BrojLicneKarte);
                        if (authorToRemove != null)
                        {
                            selectedKnjiga.Autori.Remove(authorToRemove);
                        }
                    }

                    PopuniKnjigeAutora();
                    btnUkloniKnjigu.IsEnabled = false;

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
                        int.TryParse(txtGodine.Text, out _) &&
                        txtAdresa.Text.Split(',').Length >= 3;

            btnPotvrdi.IsEnabled = isOk;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            string[] delovi = txtAdresa.Text.Split(',');
            string ulica = delovi.Length > 0 ? delovi[0].Trim() : "Nepoznato";
            string broj = delovi.Length > 1 ? delovi[1].Trim() : "/";
            string grad = delovi.Length > 2 ? delovi[2].Trim() : "Nepoznato";
            string drzava = delovi.Length > 3 ? delovi[3].Trim() : "";
            int generisaniId = (int)(DateTime.Now.Ticks % 1000000);

            IzmenjeniAutor = new Autor
            {
                BrojLicneKarte = txtBrojLicne.Text.Trim(),
                Ime = txtIme.Text.Trim(),
                Prezime = txtPrezime.Text.Trim(),
                DatumRodjenja = dpDatumRodjenja.SelectedDate ?? DateTime.Now,
                Telefon = txtTelefon.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                GodineIskustva = int.TryParse(txtGodine.Text.Trim(), out var g) ? g : 0,
                AdresaStanovanja = new Adresa(generisaniId, ulica, broj, grad, drzava),
                SpisakKnjiga = _autorKnjige
            };

            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}