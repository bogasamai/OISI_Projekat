using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public enum StatusPosetioca
    {
        R,
        V  
    }

   public class Posetilac
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public Adresa Adresa { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string BrojClanskeKarte { get; set; }
        public int GodinaClanstva { get; set; }
        public StatusPosetioca Status { get; set; }
        public double ProsecnaOcena { get; set; }

        public List<Kupovina> KupljeneKnjige { get; set; } = new List<Kupovina>();
        public List<Knjiga> ListaZelja { get; set; } = new List<Knjiga>();

        public Posetilac() { }

        public override string ToString()
        {
            return $"{Ime} {Prezime} ({BrojClanskeKarte}) - Status: {Status}, Član od: {GodinaClanstva}.";
        }

        // Display properties for UI with "nepoznato" fallback
        public string BrojClanskeKarteDisplay => string.IsNullOrWhiteSpace(BrojClanskeKarte) ? "nepoznato" : BrojClanskeKarte;
        public string ImeDisplay => string.IsNullOrWhiteSpace(Ime) ? "nepoznato" : Ime;
        public string PrezimeDisplay => string.IsNullOrWhiteSpace(Prezime) ? "nepoznato" : Prezime;
        public string AdresaDisplay => Adresa != null ? Adresa.ToString() : "nepoznato";
        public string StatusDisplay => Status.ToString() == "R" ? "Regularni" : Status.ToString() == "V" ? "VIP" : "nepoznato";
    }
}
