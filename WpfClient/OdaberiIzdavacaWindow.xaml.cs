using System.Collections.Generic;
using System.Windows;

namespace WpfClient
{
    public partial class OdaberiIzdavacaWindow : Window
    {
        public string SelectedIzdavac { get; private set; }

        public OdaberiIzdavacaWindow(List<string> izdavaci)
        {
            InitializeComponent();
            LbIzdavaci.ItemsSource = izdavaci ?? new List<string>();
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
    }
}
