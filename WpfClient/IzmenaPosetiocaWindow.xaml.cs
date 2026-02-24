using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class IzmenaPosetiocaWindow : Window
    {
        public Posetilac IzmenjeniPosetilac { get; private set; }
        private Posetilac _originalPosetilac;
        private List<Kupovina> _kupljeneKnjige;
        private List<Knjiga> _listaZelja;

        public IzmenaPosetiocaWindow(Posetilac p)
        {
            InitializeComponent();
            _originalPosetilac = p;
            _kupljeneKnjige = new List<Kupovina>(p.KupljeneKnjige ?? new List<Kupovina>());
            _listaZelja = new List<Knjiga>(p.ListaZelja ?? new List<Knjiga>());
            PopuniPolja(p);
            PopuniKupljeneKnjige();
            IzracunajStatistike();
            PopuniListuZelja();
        }

        private void PopuniPolja(Posetilac p)
        {
            txtBrojKarte.Text = p.BrojClanskeKarte;
            txtIme.Text = p.Ime;
            txtPrezime.Text = p.Prezime;
            dpDatumRodjenja.SelectedDate = p.DatumRodjenja;
            txtTelefon.Text = p.Telefon;
            txtEmail.Text = p.Email;
            txtAdresa.Text = $"{p.Adresa.Ulica}, {p.Adresa.Broj}, {p.Adresa.Grad}, {p.Adresa.Drzava}";

            foreach (ComboBoxItem item in cbStatus.Items)
            {
                if (item.Tag.ToString() == p.Status.ToString())
                    cbStatus.SelectedItem = item;
            }
        }

        private void PopuniKupljeneKnjige()
        {
            dgKupljeneKnjige.ItemsSource = null;
            dgKupljeneKnjige.ItemsSource = _kupljeneKnjige;
        }

        private void IzracunajStatistike()
        {
            if (_kupljeneKnjige == null || !_kupljeneKnjige.Any())
            {
                lblProsecnaOcena.Text = "0.0";
                lblUkupnaPotrosnja.Text = "0.00 RSD";
                return;
            }

            // Prosečna ocena - računaj samo za knjige koje imaju ocenu (ocena > 0)
            var ocenjeneKnjige = _kupljeneKnjige.Where(k => k.Ocena > 0).ToList();
            double prosecnaOcena = ocenjeneKnjige.Any() ? ocenjeneKnjige.Average(k => k.Ocena) : 0.0;
            lblProsecnaOcena.Text = prosecnaOcena.ToString("F1");

            // Ukupna potrošnja
            double ukupnaPotrosnja = _kupljeneKnjige.Sum(k => k.Knjiga?.Cena ?? 0);
            lblUkupnaPotrosnja.Text = $"{ukupnaPotrosnja:F2} RSD";
        }

        private void DgKupljeneKnjige_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnPonistiKupovinu.IsEnabled = dgKupljeneKnjige.SelectedItem != null;
        }

        private void PopuniListuZelja()
        {
            dgZelja.ItemsSource = null;
            dgZelja.ItemsSource = _listaZelja;
            var btnKup = this.FindName("BtnZeljaKupovina") as Button;
            if (btnKup != null) btnKup.IsEnabled = dgZelja.SelectedItem != null;
            // delete button (if exists) enable based on selection
            var btn = this.FindName("BtnZeljaObrisi") as Button;
            if (btn != null) btn.IsEnabled = dgZelja.SelectedItem != null;
        }

        private void DgZelja_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // enable/disable buttons if they are named
            var btnObrisi = this.FindName("BtnZeljaObrisi") as Button;
            var btnKup = this.FindName("BtnZeljaKupovina") as Button;
            if (btnObrisi != null) btnObrisi.IsEnabled = dgZelja.SelectedItem != null;
            if (btnKup != null) btnKup.IsEnabled = dgZelja.SelectedItem != null;
        }

        private void BtnZeljaDodaj_Click(object sender, RoutedEventArgs e)
        {
            // Kreiramo prozor i prosleđujemo trenutne liste radi filtriranja
            var dialog = new DodajKnjiguUZeljuWindow(_kupljeneKnjige, _listaZelja);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true && dialog.SelektovanaKnjiga != null)
            {
                // Dodavanje izabrane knjige u lokalnu listu želja
                _listaZelja.Add(dialog.SelektovanaKnjiga);

                // Ažuriranje prikaza u tabeli Želje
                PopuniListuZelja();
            }
        }

        private void BtnZeljaObrisi_Click(object sender, RoutedEventArgs e)
        {
            if (dgZelja.SelectedItem is Knjiga selected)
            {
                if (MessageBox.Show($"Da li ste sigurni da želite obrisati '{selected.Naziv}' iz liste želja?", "Potvrda", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _listaZelja.Remove(selected);
                    PopuniListuZelja();
                }
            }
        }

        private void BtnZeljaKupovina_Click(object sender, RoutedEventArgs e)
        {
            if (dgZelja.SelectedItem is Knjiga selected)
            {
                // Pretvaramo wishlist stavku u kupovinu
                var kup = new Kupovina
                {
                    Knjiga = selected,
                    DatumKupovine = DateTime.Now,
                    Ocena = 0,
                    Posetilac = _originalPosetilac
                };
                _kupljeneKnjige.Add(kup);
                _listaZelja.Remove(selected);
                PopuniKupljeneKnjige();
                PopuniListuZelja();
                IzracunajStatistike();
            }
        }

        private void BtnPonistiKupovinu_Click(object sender, RoutedEventArgs e)
        {
            if (dgKupljeneKnjige.SelectedItem is Kupovina selectedKupovina)
            {
                // Show custom modal dialog
                var dialog = new PonistiKupovinuDialog(selectedKupovina.Knjiga?.Naziv ?? "Nepoznata knjiga");
                dialog.Owner = this; // Makes it modal and centered relative to parent

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    // Remove from purchased books
                    _kupljeneKnjige.Remove(selectedKupovina);

                    // Add to wishlist 
                    // if (selectedKupovina.Knjiga != null && !_listaZelja.Any(k => k.ISBN == selectedKupovina.Knjiga.ISBN))
                    // {
                    //     _listaZelja.Add(selectedKupovina.Knjiga);
                    // }

                    // Update UI
                    PopuniKupljeneKnjige();
                    IzracunajStatistike();
                    btnPonistiKupovinu.IsEnabled = false;
                }
            }
        }

        private void ValidateForm(object sender, EventArgs e)
        {
            if (btnPotvrdi == null) return;

            bool isOk = !string.IsNullOrWhiteSpace(txtBrojKarte.Text) &&
                        !string.IsNullOrWhiteSpace(txtIme.Text) &&
                        !string.IsNullOrWhiteSpace(txtPrezime.Text) &&
                        txtEmail.Text.Contains("@") &&
                        txtAdresa.Text.Split(',').Length >= 3;

            btnPotvrdi.IsEnabled = isOk;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Logika za parsiranje adrese je ista kao u tvom DodajPosetiocaWindow
            string[] delovi = txtAdresa.Text.Split(',');
            Adresa novaAdresa = new Adresa(0, delovi[0].Trim(), delovi[1].Trim(), delovi[2].Trim(), delovi.Length > 3 ? delovi[3].Trim() : "");

            IzmenjeniPosetilac = new Posetilac
            {
                BrojClanskeKarte = txtBrojKarte.Text,
                Ime = txtIme.Text,
                Prezime = txtPrezime.Text,
                DatumRodjenja = dpDatumRodjenja.SelectedDate ?? DateTime.Now,
                Telefon = txtTelefon.Text,
                Email = txtEmail.Text,
                Adresa = novaAdresa,
                Status = (cbStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() == "V" ? StatusPosetioca.V : StatusPosetioca.R,
                KupljeneKnjige = _kupljeneKnjige, // Include modified purchases
                ListaZelja = _listaZelja // Include modified wishlist
            };

            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}

