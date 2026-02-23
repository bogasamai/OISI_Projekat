using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfClient
{
    public partial class DodajKnjiguWindow : Window
    {
        // Property koji će glavni prozor pročitati nakon zatvaranja
        public Knjiga NovaKnjiga { get; private set; }

        public DodajKnjiguWindow()
        {
            InitializeComponent();
        }

        private void ValidateForm(object sender, EventArgs e)
        {
            // 1. Provera da li su obavezna polja popunjena
            bool osnovnaPoljaPopunjena = !string.IsNullOrWhiteSpace(txtISBN.Text) &&
                                         !string.IsNullOrWhiteSpace(txtNaziv.Text) &&
                                         !string.IsNullOrWhiteSpace(txtGodina.Text) &&
                                         !string.IsNullOrWhiteSpace(txtCena.Text);

            // 2. Provera validnosti brojeva (Godina mora biti int, Cena mora biti double)
            bool godinaValidna = int.TryParse(txtGodina.Text, out int godina) && godina > 0 && godina <= DateTime.Now.Year;
            bool cenaValidna = double.TryParse(txtCena.Text, out double cena) && cena >= 0;

            // 3. Provera broja strana (opciono polje, ali ako se unese mora biti broj)
            bool brojStranaValidan = true;
            if (!string.IsNullOrWhiteSpace(txtBrojStrana.Text))
            {
                brojStranaValidan = int.TryParse(txtBrojStrana.Text, out _);
            }

            // 4. ISBN validacija (obično 10 ili 13 cifara, ovde proveravamo samo da li su cifre)
            //bool isbnValidan = txtISBN.Text.All(char.IsDigit) && (txtISBN.Text.Length == 10 || txtISBN.Text.Length == 13);
            bool isbnValidan = txtISBN.Text.All(char.IsDigit);
            // Omogući dugme samo ako je sve ispravno
            btnPotvrdi.IsEnabled = osnovnaPoljaPopunjena && godinaValidna && cenaValidna && brojStranaValidan && isbnValidan;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kreiranje objekta na osnovu unetih podataka
                NovaKnjiga = new Knjiga
                {
                    ISBN = txtISBN.Text.Trim(),
                    Naziv = txtNaziv.Text.Trim(),
                    Zanr = txtZanr.Text.Trim(),
                    GodinaIzdanja = int.Parse(txtGodina.Text),
                    Cena = double.Parse(txtCena.Text),
                    BrojStrana = string.IsNullOrWhiteSpace(txtBrojStrana.Text) ? 0 : int.Parse(txtBrojStrana.Text),
                    Izdavac = txtIzdavac.Text.Trim(),
                    Autori = new List<Autor>(), // Autori se obično dodaju u drugom koraku ili preko posebne selekcije
                    PosetiociKupili = new List<Posetilac>(),
                    PosetiociListaZelja = new List<Posetilac>()
                };

                DialogResult = true; // Zatvara prozor i signalizira uspeh
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri kreiranju knjige: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}