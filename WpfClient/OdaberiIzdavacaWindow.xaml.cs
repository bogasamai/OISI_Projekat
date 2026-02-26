using System.Collections.Generic;
using System.Windows;

namespace WpfClient
{
    public partial class OdaberiIzdavacaWindow : Window
    {
        public string SelectedIzdavac { get; private set; }
        private List<Core.Models.Knjiga> _sveKnjige;

        public OdaberiIzdavacaWindow(List<string> izdavaci, List<Core.Models.Knjiga> sveKnjige = null)
        {
            InitializeComponent();
            LbIzdavaci.ItemsSource = izdavaci ?? new List<string>();
            _sveKnjige = sveKnjige ?? new List<Core.Models.Knjiga>();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            SelectedIzdavac = LbIzdavaci.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(SelectedIzdavac))
            {
                MessageBox.Show("Molimo izaberite izdavača.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnShowBooks_Click(object sender, RoutedEventArgs e)
        {
            var izd = LbIzdavaci.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(izd))
            {
                MessageBox.Show("Molimo izaberite izdavača iz liste.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var wnd = new BooksForPublisherWindow(izd, _sveKnjige);
            wnd.Owner = this;
            wnd.ShowDialog();
        }
    }
}
