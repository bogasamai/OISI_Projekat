using Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class IzmenaAutoraWindow : Window
    {
        public Autor IzmenjeniAutor { get; private set; }

        public IzmenaAutoraWindow(Autor a)
        {
            InitializeComponent();
            PopuniPolja(a);
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
                AdresaStanovanja = new Adresa { Ulica = txtAdresa.Text.Trim() }
            };

            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
