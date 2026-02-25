using Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class AutoriZaPosetiocaWindow : Window
    {
        private List<Autor> _allAuthors = new List<Autor>();
        private List<Autor> _visibleAuthors = new List<Autor>();

        public AutoriZaPosetiocaWindow(Posetilac posetilac)
        {
            InitializeComponent();
            if (posetilac == null) return;

            // Collect unique authors from books in wishlist
            var authors = new List<Autor>();
            if (posetilac.ListaZelja != null)
            {
                foreach (var knj in posetilac.ListaZelja)
                {
                    if (knj?.Autori == null) continue;
                    foreach (var a in knj.Autori)
                    {
                        if (a == null) continue;
                        if (!authors.Any(x => x.BrojLicneKarte == a.BrojLicneKarte))
                            authors.Add(a);
                    }
                }
            }

            _allAuthors = authors;
            _visibleAuthors = _allAuthors.ToList();
            dgAutori.ItemsSource = _visibleAuthors;
        }

        private void TxtPretraga_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = txtPretraga.Text?.Trim().ToLower() ?? string.Empty;
            if (string.IsNullOrEmpty(q))
            {
                _visibleAuthors = _allAuthors.ToList();
            }
            else
            {
                _visibleAuthors = _allAuthors.Where(a =>
                    (a.Ime ?? string.Empty).ToLower().Contains(q) ||
                    (a.Prezime ?? string.Empty).ToLower().Contains(q) ||
                    (a.Email ?? string.Empty).ToLower().Contains(q)
                ).ToList();
            }
            dgAutori.ItemsSource = null;
            dgAutori.ItemsSource = _visibleAuthors;
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
