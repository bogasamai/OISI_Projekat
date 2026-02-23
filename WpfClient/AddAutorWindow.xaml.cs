using Core.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace WpfClient
{
    public partial class AddAutorWindow : Window
    {
        public Autor Result { get; private set; }

        public AddAutorWindow()
        {
            InitializeComponent();

            TxtIme.TextChanged += ValidateForm;
            TxtPrezime.TextChanged += ValidateForm;
            DpDatum.SelectedDateChanged += (s, e) => ValidateForm(s, null);
            TxtBrojLicne.TextChanged += ValidateForm;
            TxtEmail.TextChanged += ValidateForm;
            TxtGodine.TextChanged += ValidateForm;

            DpDatum.SelectedDate = DateTime.Now;
            ValidateForm(this, null);
        }

        private void ValidateForm(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            if (string.IsNullOrWhiteSpace(TxtIme.Text)) valid = false;
            if (string.IsNullOrWhiteSpace(TxtPrezime.Text)) valid = false;
            if (!DpDatum.SelectedDate.HasValue) valid = false;
            if (string.IsNullOrWhiteSpace(TxtBrojLicne.Text)) valid = false;
            if (string.IsNullOrWhiteSpace(TxtEmail.Text) || !Regex.IsMatch(TxtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) valid = false;
            if (!int.TryParse(TxtGodine.Text, out _)) valid = false;

            BtnConfirm.IsEnabled = valid;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Result = new Autor
            {
                Ime = TxtIme.Text.Trim(),
                Prezime = TxtPrezime.Text.Trim(),
                BrojLicneKarte = TxtBrojLicne.Text.Trim(),
                DatumRodjenja = DpDatum.SelectedDate ?? DateTime.Now,
                Telefon = TxtTelefon.Text.Trim(),
                Email = TxtEmail.Text.Trim(),
                GodineIskustva = int.TryParse(TxtGodine.Text.Trim(), out var g) ? g : 0,
                AdresaStanovanja = new Adresa { Ulica = TxtAdresa.Text.Trim() }
            };

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}