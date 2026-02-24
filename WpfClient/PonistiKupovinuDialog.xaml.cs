using System.Windows;

namespace WpfClient
{
    public partial class PonistiKupovinuDialog : Window
    {
        public PonistiKupovinuDialog(string nazivKnjige)
        {
            InitializeComponent();
            txtMessage.Text = $"Da li ste sigurni da želite da poništite kupovinu knjige '{nazivKnjige}'?\n\nNakon poništavanja, knjiga će biti prebačena u listu želja.";
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

