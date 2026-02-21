using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
   public class Autor
{
    public string Ime { get; set; } //IM
    public string Prezime { get; set; } //PR
    public DateTime DatumRodjenja { get; set; } //DR
    public Adresa AdresaStanovanja { get; set; } 
    public string Telefon { get; set; } // TEL
    public string Email { get; set; }
    public string BrojLicneKarte { get; set; } //LK
    public int GodineIskustva { get; set; }
    public List<Knjiga> SpisakKnjiga { get; set; } 

    public Autor()
    {
        SpisakKnjiga = new List<Knjiga>();
    }

        public override string ToString()
        {
            return $"Autor: {Ime} {Prezime} | LK: {BrojLicneKarte} | Email: {Email} | Iskustvo: {GodineIskustva} god.";
        }

        // Convenience read-only properties for UI binding
        public string PunoIme => $"{Ime} {Prezime}";
        public int BrojKnjiga => SpisakKnjiga?.Count ?? 0;

        // Display properties for UI with "nepoznato" fallback
        public string ImeDisplay => string.IsNullOrWhiteSpace(Ime) ? "nepoznato" : Ime;
        public string PrezimeDisplay => string.IsNullOrWhiteSpace(Prezime) ? "nepoznato" : Prezime;
        public string BrojLicneKarteDisplay => string.IsNullOrWhiteSpace(BrojLicneKarte) ? "nepoznato" : BrojLicneKarte;
        public string DatumRodjenjaDisplay => DatumRodjenja == default(DateTime) ? "nepoznato" : DatumRodjenja.ToString("dd.MM.yyyy.");
        public string EmailDisplay => string.IsNullOrWhiteSpace(Email) ? "nepoznato" : Email;
    }
}