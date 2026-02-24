using Core.Models;
using System;
using System.Windows;

namespace WpfClient
{
    public partial class IzmenaKnjigeWindow : Window
    {
        public Knjiga IzmenjenaKnjiga { get; private set; }
        private Knjiga _originalKnjiga;

        public IzmenaKnjigeWindow(Knjiga knjigaZaIzmenu)
        {
            InitializeComponent();

            _originalKnjiga = knjigaZaIzmenu;

            // Popuni polja postojećim podacima
            txtISBN.Text = knjigaZaIzmenu.ISBN;
            txtNaziv.Text = knjigaZaIzmenu.Naziv;
            txtZanr.Text = knjigaZaIzmenu.Zanr;
            txtGodina.Text = knjigaZaIzmenu.GodinaIzdanja.ToString();
            txtCena.Text = knjigaZaIzmenu.Cena.ToString();
            txtBrojStrana.Text = knjigaZaIzmenu.BrojStrana > 0 ? knjigaZaIzmenu.BrojStrana.ToString() : "";
            txtIzdavac.Text = knjigaZaIzmenu.Izdavac;

            // Dodaj validaciju na promenu polja
            txtISBN.TextChanged += ValidateForm;
            txtNaziv.TextChanged += ValidateForm;
            txtZanr.TextChanged += ValidateForm;
            txtGodina.TextChanged += ValidateForm;
            txtCena.TextChanged += ValidateForm;
            txtBrojStrana.TextChanged += ValidateForm;
            txtIzdavac.TextChanged += ValidateForm;

            ValidateForm(null, null);
        }

        private void ValidateForm(object sender, EventArgs e)
        {
            bool osnovnaPoljaPopunjena = !string.IsNullOrWhiteSpace(txtISBN.Text) &&
                                            !string.IsNullOrWhiteSpace(txtNaziv.Text) &&
                                            !string.IsNullOrWhiteSpace(txtGodina.Text) &&
                                            !string.IsNullOrEmpty(txtZanr.Text) &&
                                            !string.IsNullOrWhiteSpace(txtCena.Text) &&
                                            !string.IsNullOrWhiteSpace(txtBrojStrana.Text); // BrojStrana mora biti popunjen


            bool godinaValidna = int.TryParse(txtGodina.Text, out int godina) && godina > 0 && godina <= DateTime.Now.Year;
            bool cenaValidna = double.TryParse(txtCena.Text, out double cena) && cena >= 0;

            bool brojStranaValidan = int.TryParse(txtBrojStrana.Text, out int brojStrana) && brojStrana > 0; // Mora biti broj i > 0


            bool isbnValidan = txtISBN.Text.All(char.IsDigit);

            btnPotvrdi.IsEnabled = osnovnaPoljaPopunjena && godinaValidna && cenaValidna && brojStranaValidan && isbnValidan;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IzmenjenaKnjiga = new Knjiga
                {
                    ISBN = txtISBN.Text.Trim(),
                    Naziv = txtNaziv.Text.Trim(),
                    Zanr = txtZanr.Text.Trim(),
                    GodinaIzdanja = int.Parse(txtGodina.Text),
                    Cena = double.Parse(txtCena.Text),
                    BrojStrana = string.IsNullOrWhiteSpace(txtBrojStrana.Text) ? 0 : int.Parse(txtBrojStrana.Text),
                    Izdavac = txtIzdavac.Text.Trim(),
                    Autori = _originalKnjiga.Autori, // Autori ostaju isti
                    PosetiociKupili = _originalKnjiga.PosetiociKupili,
                    PosetiociListaZelja = _originalKnjiga.PosetiociListaZelja
                };

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri izmeni knjige: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
