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

        public IzmenaPosetiocaWindow(Posetilac p)
        {
            InitializeComponent();
            _originalPosetilac = p;
            _kupljeneKnjige = new List<Kupovina>(p.KupljeneKnjige ?? new List<Kupovina>());
            PopuniPolja(p);
            PopuniKupljeneKnjige();
            IzracunajStatistike();
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

        private void BtnPonistiKupovinu_Click(object sender, RoutedEventArgs e)
        {
            if (dgKupljeneKnjige.SelectedItem is Kupovina selectedKupovina)
            {
                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da poništite kupovinu knjige '{selectedKupovina.Knjiga?.Naziv}'?",
                    "Potvrda brisanja",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _kupljeneKnjige.Remove(selectedKupovina);
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
                ListaZelja = _originalPosetilac.ListaZelja // Keep original wishlist
            };

            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
