using Core.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace WpfClient
{
    public partial class DodajKupovinuWindow : Window
    {
        public Kupovina Result { get; private set; }
        private Knjiga _knjiga;

        public DodajKupovinuWindow(Knjiga knjiga)
        {
            InitializeComponent();
            _knjiga = knjiga ?? throw new ArgumentNullException(nameof(knjiga));
            txtISBN.Text = _knjiga.ISBN;
            txtNaziv.Text = _knjiga.Naziv;
            dpDatum.SelectedDate = DateTime.Now;
        }

        private void TxtOcena_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]$");
        }

        private void TxtOcena_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateConfirmState();
        }

        private void DpDatum_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateConfirmState();
        }

        private void UpdateConfirmState()
        {
            bool ok = dpDatum.SelectedDate.HasValue && int.TryParse(txtOcena.Text, out int oc) && oc >= 1 && oc <= 5;
            btnPotvrdi.IsEnabled = ok;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            Result = new Kupovina
            {
                Knjiga = _knjiga,
                DatumKupovine = dpDatum.SelectedDate ?? DateTime.Now,
                Ocena = int.TryParse(txtOcena.Text, out int oc) ? oc : 0,
                Komentar = string.Empty
            };
            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
