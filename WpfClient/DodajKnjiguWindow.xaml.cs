using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfClient
{
    public partial class DodajKnjiguWindow : Window
    {
        public Knjiga NovaKnjiga { get; private set; }

        public DodajKnjiguWindow()
        {
            InitializeComponent();
        }

        private void ValidateForm(object sender, EventArgs e)
        {
           
            bool osnovnaPoljaPopunjena = !string.IsNullOrWhiteSpace(txtISBN.Text) &&
                                            !string.IsNullOrWhiteSpace(txtNaziv.Text) &&
                                            !string.IsNullOrWhiteSpace(txtGodina.Text) &&
                                            !string.IsNullOrEmpty(txtZanr.Text) &&
                                            !string.IsNullOrWhiteSpace(txtCena.Text) &&
                                            !string.IsNullOrWhiteSpace(txtBrojStrana.Text); 


           
            bool godinaValidna = int.TryParse(txtGodina.Text, out int godina) && godina > 0 && godina <= DateTime.Now.Year;
            bool cenaValidna = double.TryParse(txtCena.Text, out double cena) && cena >= 0;


            bool brojStranaValidan = int.TryParse(txtBrojStrana.Text, out int brojStrana) && brojStrana > 0;


            bool isbnValidan = txtISBN.Text.All(char.IsDigit);
            btnPotvrdi.IsEnabled = osnovnaPoljaPopunjena && godinaValidna && cenaValidna && brojStranaValidan && isbnValidan;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NovaKnjiga = new Knjiga
                {
                    ISBN = txtISBN.Text.Trim(),
                    Naziv = txtNaziv.Text.Trim(),
                    Zanr = txtZanr.Text.Trim(),
                    GodinaIzdanja = int.Parse(txtGodina.Text),
                    Cena = double.Parse(txtCena.Text),
                    BrojStrana = string.IsNullOrWhiteSpace(txtBrojStrana.Text) ? 0 : int.Parse(txtBrojStrana.Text),
                    Izdavac = txtIzdavac.Text.Trim(),
                    Autori = new List<Autor>(), 
                    PosetiociKupili = new List<Posetilac>(),
                    PosetiociListaZelja = new List<Posetilac>()
                };

                DialogResult = true;
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