using Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfClient
{
    public partial class DodajPosetiocaWindow : Window
    {
        public Posetilac NoviPosetilac { get; private set; }

        public DodajPosetiocaWindow()
        {
            InitializeComponent();
        }
        private void ValidateForm(object sender, EventArgs e)
        {
            bool svaPoljaPopunjena = !string.IsNullOrWhiteSpace(txtBrojKarte.Text) &&
                                     !string.IsNullOrWhiteSpace(txtIme.Text) &&
                                     !string.IsNullOrWhiteSpace(txtPrezime.Text) &&
                                     !string.IsNullOrWhiteSpace(txtEmail.Text) &&
                                     !string.IsNullOrWhiteSpace(txtAdresa.Text) &&
                                     !string.IsNullOrWhiteSpace(txtTelefon.Text) &&
                                     dpDatumRodjenja.SelectedDate != null;

            // 2. Ime i prezime ne smeju imati brojeve
            bool imeValidno = txtIme.Text.All(c => !char.IsDigit(c));
            bool prezimeValidno = txtPrezime.Text.All(c => !char.IsDigit(c));

            // 3. Telefon ne sme imati slova, dozvoljen je samo '+' i cifre
            // (Proveravamo da li su svi karakteri ili cifre ili znak plus)
            bool telefonValidan = txtTelefon.Text.All(c => char.IsDigit(c) || c == '+');

            // Dugme "Potvrdi" se omogućava samo ako su SVI uslovi ispunjeni
            bool adresaImaZareze = txtAdresa.Text.Split(',').Length >= 3;

            btnPotvrdi.IsEnabled = svaPoljaPopunjena && imeValidno && prezimeValidno && telefonValidan&& adresaImaZareze;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtBrojKarte.Text) || string.IsNullOrWhiteSpace(txtIme.Text))
            {
                MessageBox.Show("Sva polja sa zvezdicom su obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var selectedItem = cbStatus.SelectedItem as ComboBoxItem;
            string statusTag = selectedItem?.Tag?.ToString(); // Vratiće "R" ili "V"
            string rawAdresa = txtAdresa.Text;

            // Delimo string po zarezu
            string[] delovi = rawAdresa.Split(',');

            // Pripremamo varijable (trimujemo razmake da ne bude " Beograd")
            string ulica = delovi.Length > 0 ? delovi[0].Trim() : "Nepoznato";
            string broj = delovi.Length > 1 ? delovi[1].Trim() : "/";
            string grad = delovi.Length > 2 ? delovi[2].Trim() : "Nepoznato";
            string drzava = delovi.Length > 3 ? delovi[3].Trim() : "";
            // Za Id koristimo npr. milisekunde trenutnog vremena da bude unikatan (privremeno rešenje)
            int generisaniId = (int)(DateTime.Now.Ticks % 1000000);
            Adresa unesenaAdresa = new Adresa(generisaniId, ulica, broj, grad, drzava);
            NoviPosetilac = new Posetilac
            {
                BrojClanskeKarte=txtBrojKarte.Text,
                Ime= txtIme.Text,
                Prezime = txtPrezime.Text,
                DatumRodjenja = dpDatumRodjenja.SelectedDate ?? DateTime.Now,
                Telefon = txtTelefon.Text,
                Email = txtEmail.Text,
                Adresa = unesenaAdresa,
                Status = Enum.TryParse(typeof(StatusPosetioca), statusTag, out var s) ? (StatusPosetioca)s : StatusPosetioca.R
            };

            DialogResult = true; // Zatvara prozor i vraća potvrdu
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}