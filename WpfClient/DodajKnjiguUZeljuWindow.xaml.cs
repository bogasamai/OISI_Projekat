using Core.Models;
using Core.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfClient
{
    public partial class DodajKnjiguUZeljuWindow : Window
    {
        public Knjiga SelektovanaKnjiga { get; private set; }

        public DodajKnjiguUZeljuWindow(List<Kupovina> kupljene, List<Knjiga> zelje)
        {
            InitializeComponent();
            FiltrirajIPopuni(kupljene, zelje);
        }

        private void FiltrirajIPopuni(List<Kupovina> kupljene, List<Knjiga> zelje)
        {
            // Učitavamo sve dostupne knjige iz sistema
            List<Knjiga> sveKnjige = DataHandler.UcitajKnjige();

            // Dobijamo set ISBN-ova knjiga koje posetilac već poseduje ili želi
            var kupljeniIsbn = kupljene.Select(k => k.Knjiga.ISBN).ToList();
            var zeljeIsbn = zelje.Select(z => z.ISBN).ToList();

            // Uslov: Knjiga se ne nalazi ni u listi kupljenih, ni u listi želja
            var filtriraneKnjige = sveKnjige.Where(k =>
                !kupljeniIsbn.Contains(k.ISBN) &&
                !zeljeIsbn.Contains(k.ISBN)
            ).ToList();

            lbKnjige.ItemsSource = filtriraneKnjige;
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            if (lbKnjige.SelectedItem is Knjiga izabrana)
            {
                SelektovanaKnjiga = izabrana;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Morate izabrati knjigu iz liste.", "Napomena", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}