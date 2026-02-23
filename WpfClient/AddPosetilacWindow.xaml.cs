using Core.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class AddPosetilacWindow : Window
    {
        public Posetilac Result { get; private set; }

        public AddPosetilacWindow()
        {
            InitializeComponent();

            TxtIme.TextChanged += ValidateForm;
            TxtPrezime.TextChanged += ValidateForm;
            DpDatum.SelectedDateChanged += (s, e) => ValidateForm(s, null);
            TxtTelefon.TextChanged += ValidateForm;
            TxtEmail.TextChanged += ValidateForm;
            TxtBrojClanske.TextChanged += ValidateForm;
            CbStatus.SelectionChanged += (s, e) => ValidateForm(s, null);

            // Set sensible defaults so form is easier to complete
            DpDatum.SelectedDate = DateTime.Now;
            if (CbStatus.Items.Count > 0) CbStatus.SelectedIndex = 0;

            // Evaluate validity initially
            ValidateForm(this, null);

            BtnConfirm.IsEnabled = false;
        }

        private void ValidateForm(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            // Required: Ime, Prezime, Datum, BrojClanske, GodinaClanstva (int), Status, Email
            if (string.IsNullOrWhiteSpace(TxtIme.Text)) valid = false;
            if (string.IsNullOrWhiteSpace(TxtPrezime.Text)) valid = false;
            if (!DpDatum.SelectedDate.HasValue) valid = false;
            if (string.IsNullOrWhiteSpace(TxtBrojClanske.Text)) valid = false;
            if (CbStatus.SelectedItem == null) valid = false;
            // basic email check
            if (string.IsNullOrWhiteSpace(TxtEmail.Text) || !Regex.IsMatch(TxtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) valid = false;

            BtnConfirm.IsEnabled = valid;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Result = new Posetilac
            {
                Ime = TxtIme.Text.Trim(),
                Prezime = TxtPrezime.Text.Trim(),
                BrojClanskeKarte = TxtBrojClanske.Text.Trim(),
                DatumRodjenja = DpDatum.SelectedDate ?? DateTime.Now,
                Telefon = TxtTelefon.Text.Trim(),
                Email = TxtEmail.Text.Trim(),
                Status = (CbStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() == "V" ? Core.Models.StatusPosetioca.V : Core.Models.StatusPosetioca.R,
                Adresa = new Adresa { Ulica = TxtAdresa.Text.Trim() }
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