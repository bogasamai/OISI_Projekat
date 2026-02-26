using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class AutoriZaIzdavacaWindow : Window
    {
        private List<Autor> _allAuthors = new List<Autor>();
        private List<Autor> _visibleAuthors = new List<Autor>();

        public AutoriZaIzdavacaWindow(string izdavacNaziv, List<Knjiga> sveKnjige)
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(izdavacNaziv) || sveKnjige == null) return;

            // Pronađi sve knjige koje imaju polje Izdavac == izdavacNaziv
            var knjigeIzdavaca = sveKnjige.Where(k => string.Equals(k.Izdavac?.Trim(), izdavacNaziv.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            var authors = new List<Autor>();
            foreach (var knj in knjigeIzdavaca)
            {
                if (knj?.Autori == null) continue;
                foreach (var a in knj.Autori)
                {
                    if (a == null) continue;
                    if (!authors.Any(x => x.BrojLicneKarte == a.BrojLicneKarte))
                        authors.Add(a);
                }
            }

            _allAuthors = authors;
            _visibleAuthors = _allAuthors.ToList();
            dgAutori.ItemsSource = _visibleAuthors;
            this.Title = $"Autori izdavača: {izdavacNaziv}";
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
                    (a.Email ?? string.Empty).ToLower().Contains(q) ||
                    (a.AdresaStanovanja?.Ulica ?? string.Empty).ToLower().Contains(q) ||
                    (a.Telefon ?? string.Empty).ToLower().Contains(q)
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
