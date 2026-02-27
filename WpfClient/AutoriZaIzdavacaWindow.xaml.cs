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

        private Izdavac _trenutniIzdavac;

        public AutoriZaIzdavacaWindow(Izdavac izdavac, List<Knjiga> sveKnjige)
        {
            InitializeComponent();

            if (izdavac == null || sveKnjige == null) return;

            _trenutniIzdavac = izdavac;
            string izdavacNaziv = izdavac.Naziv;

            // Pronalaženje svih autora koji su radili za ovog izdavača
            var knjigeIzdavaca = sveKnjige
                .Where(k => string.Equals(k.Izdavac?.Trim(), izdavacNaziv.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

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
            OsveziPrikazSefa();
            this.Title = $"Autori izdavača: {izdavacNaziv}";

            // Inicijalno onemogući dugme dok se ne selektuje neko
            btnPostaviZaSefa.IsEnabled = false;
        }
        private void OsveziPrikazSefa()
        {
            if (_trenutniIzdavac.SefIzdavaca != null)
            {
                lblTrenutniSef.Text = $"{_trenutniIzdavac.SefIzdavaca.Ime} {_trenutniIzdavac.SefIzdavaca.Prezime}";
            }
            else
            {
                lblTrenutniSef.Text = "Nije postavljen"; 
            }
        }
        private void TxtPretraga_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = txtPretraga.Text?.Trim().ToLower() ?? string.Empty;
            _visibleAuthors = string.IsNullOrEmpty(q)
                ? _allAuthors.ToList()
                : _allAuthors.Where(a =>
                    (a.Ime ?? "").ToLower().Contains(q) ||
                    (a.Prezime ?? "").ToLower().Contains(q) ||
                    (a.Email ?? "").ToLower().Contains(q) ||
                    a.GodineIskustva.ToString().Contains(q)
                ).ToList();

            dgAutori.ItemsSource = null;
            dgAutori.ItemsSource = _visibleAuthors;
        }

        private void DgAutori_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAutori.SelectedItem is Autor selektovan)
            {
                // Automatska provera uslova: iskustvo >= 5
                btnPostaviZaSefa.IsEnabled = selektovan.GodineIskustva >= 5;
            }
            else
            {
                btnPostaviZaSefa.IsEnabled = false;
            }
        }

        private void BtnPostaviZaSefa_Click(object sender, RoutedEventArgs e)
        {
            if (dgAutori.SelectedItem is Autor selektovaniAutor)
            {
                // Provera biznis pravila: GodineIskustva >=5
                if (selektovaniAutor.GodineIskustva >= 5)
                {
                    //  Menjamo šefa u memoriji
                    _trenutniIzdavac.SefIzdavaca = selektovaniAutor;
                    OsveziPrikazSefa();
                    MessageBox.Show($"Autor {selektovaniAutor.Ime} {selektovaniAutor.Prezime} je uspešno postavljen za šefa izdavača '{_trenutniIzdavac.Naziv}'.",
                                    "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

                   // DialogResult = true;
                }
                else
                {
                    // Ovo je "double-check" ako neko nekako klikne na onemogućeno dugme
                    MessageBox.Show($"Greška: Autor nema dovoljno iskustva.",
                                    "Nedovoljno iskustva", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}