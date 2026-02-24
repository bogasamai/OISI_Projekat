using Core.Models;
using System.Collections.Generic;
using System.Windows;

namespace WpfClient
{
    public partial class ChooseAutorWindow : Window
    {
        public Autor SelectedAuthor { get; private set; }

        public ChooseAutorWindow(List<Autor> autori)
        {
            InitializeComponent();
            LbAutori.ItemsSource = autori ?? new List<Autor>();
        }

        private void BtnChoose_Click(object sender, RoutedEventArgs e)
        {
            SelectedAuthor = LbAutori.SelectedItem as Autor;
            if (SelectedAuthor == null)
            {
                MessageBox.Show("Molimo izaberite autora.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
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
