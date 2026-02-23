using Core.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class IzmenaPosetiocaWindow : Window
    {
        public Posetilac IzmenjeniPosetilac { get; private set; }

        public IzmenaPosetiocaWindow(Posetilac p)
        {
            InitializeComponent();
            PopuniPolja(p);
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
                Status = (cbStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() == "V" ? StatusPosetioca.V : StatusPosetioca.R
            };

            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}