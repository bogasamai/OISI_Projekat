using Core.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class OdaberiKnjiguDialog : Window
    {
        public Knjiga OdabranaKnjiga { get; private set; }

        public OdaberiKnjiguDialog(List<Knjiga> dostupneKnjige)
        {
            InitializeComponent();
            lbDostupneKnjige.ItemsSource = dostupneKnjige;
        }

        private void LbDostupneKnjige_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnPotvrdi.IsEnabled = lbDostupneKnjige.SelectedItem != null;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            OdabranaKnjiga = lbDostupneKnjige.SelectedItem as Knjiga;
            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
