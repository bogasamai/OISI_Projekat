using Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class BooksForPublisherWindow : Window
    {
        private List<Knjiga> _all = new List<Knjiga>();
        private List<Knjiga> _visible = new List<Knjiga>();
        private string _publisher;

        public BooksForPublisherWindow(string publisher, List<Knjiga> sveKnjige)
        {
            InitializeComponent();
            _publisher = publisher;
            if (sveKnjige == null) sveKnjige = new List<Knjiga>();

            _all = sveKnjige.Where(k => string.Equals(k.Izdavac?.Trim(), publisher?.Trim(), System.StringComparison.OrdinalIgnoreCase)).ToList();
            _visible = _all.ToList();
            dgKnjige.ItemsSource = _visible;
            this.Title = $"Knjige izdavača: {publisher}";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var q = txtSearch.Text?.Trim().ToLower() ?? string.Empty;
            if (string.IsNullOrEmpty(q)) _visible = _all.ToList();
            else
            {
                _visible = _all.Where(k =>
                    (k.Naziv ?? string.Empty).ToLower().Contains(q) ||
                    (k.ISBN ?? string.Empty).ToLower().Contains(q) ||
                    (k.Zanr ?? string.Empty).ToLower().Contains(q)
                ).ToList();
            }
            dgKnjige.ItemsSource = null;
            dgKnjige.ItemsSource = _visible;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
