using Core.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class OdaberiKnjiguDialog : Window
    {
        public Knjiga OdabranaKnjiga { get; private set; }
        public List<Knjiga> OdabraneKnjige { get; private set; } = new List<Knjiga>();

        public OdaberiKnjiguDialog(List<Knjiga> dostupneKnjige)
        {
            InitializeComponent();
            lbDostupneKnjige.ItemsSource = dostupneKnjige;
        }

        private void LbDostupneKnjige_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnPotvrdi.IsEnabled = lbDostupneKnjige.SelectedItems.Count > 0;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            OdabraneKnjige = lbDostupneKnjige.SelectedItems.Cast<Knjiga>().ToList();
            OdabranaKnjiga = OdabraneKnjige.FirstOrDefault();
            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void ChkMultipleSelection_Checked(object sender, RoutedEventArgs e)
        {
            lbDostupneKnjige.SelectionMode = SelectionMode.Multiple;
        }

        private void ChkMultipleSelection_Unchecked(object sender, RoutedEventArgs e)
        {
            lbDostupneKnjige.SelectionMode = SelectionMode.Single;
            // izbrisi svu selekciju sem prve
            if (lbDostupneKnjige.SelectedItems.Count > 1)
            {
                var first = lbDostupneKnjige.SelectedItems[0];
                lbDostupneKnjige.SelectedItems.Clear();
                lbDostupneKnjige.SelectedItem = first;
            }
        }
    }
}
