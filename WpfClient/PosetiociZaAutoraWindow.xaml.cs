using Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class PosetiociZaAutoraWindow : Window
    {
        // Liste za čuvanje podataka i filtriranje
        private List<Posetilac> _allPosetioci;
        private List<Posetilac> _visiblePosetioci;

        public PosetiociZaAutoraWindow(Autor izabraniAutor, List<Posetilac> sviPosetioci)
        {
            InitializeComponent();
            if (izabraniAutor == null || sviPosetioci == null) return;

            this.Title = $"Posetioci za: {izabraniAutor.Ime} {izabraniAutor.Prezime}";

            // Sada koristimo listu koju smo dobili spolja
            var pronadjeniPosetioci = sviPosetioci.Where(p =>
                p.ListaZelja != null &&
                p.ListaZelja.Any(k => k.Autori != null &&
                                     k.Autori.Any(a => a.BrojLicneKarte == izabraniAutor.BrojLicneKarte))
            ).ToList();

            _allPosetioci = pronadjeniPosetioci;
            _visiblePosetioci = _allPosetioci.ToList();
            dgPosetioci.ItemsSource = _visiblePosetioci;
        }
        // Dodajemo i logiku za tekstualnu pretragu 
        private void txtPretraga_TextChanged(object sender, TextChangedEventArgs e)
        {
            string kriterijum = txtPretraga.Text.ToLower();

            _visiblePosetioci = _allPosetioci.Where(p =>
                p.Ime.ToLower().Contains(kriterijum) ||
                p.Prezime.ToLower().Contains(kriterijum) ||
                p.Email.ToLower().Contains(kriterijum)
            ).ToList();

            dgPosetioci.ItemsSource = _visiblePosetioci;
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}