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
        public class Knjiga
        {
        public string ISBN { get; set; }
        public string Naziv { get; set; }
        public string Zanr { get; set; } = string.Empty;
        public int GodinaIzdanja { get; set; }
        public double Cena { get; set; }
        public int BrojStrana { get; set; }
        public List<Autor> Autori { get; set; } = new List<Autor>();

       

        public string Izdavac { get; set; } = string.Empty;
        public List<Posetilac> PosetiociKupili { get; set; } = new List<Posetilac>();
        public List<Posetilac> PosetiociListaZelja { get; set; } = new List<Posetilac>();

        public Knjiga()
        {
            PosetiociKupili = new List<Posetilac>();
            PosetiociListaZelja = new List<Posetilac>();
        }

        // Display properties for UI with "nepoznato" fallback
        public string ISBNDisplay => string.IsNullOrWhiteSpace(ISBN) ? "nepoznato" : ISBN;
        public string NazivDisplay => string.IsNullOrWhiteSpace(Naziv) ? "nepoznato" : Naziv;
        public string CenaDisplay => Cena <= 0 ? "nepoznato" : Cena.ToString("F2");
        public string GodinaIzdanjaDisplay => GodinaIzdanja <= 0 ? "nepoznato" : GodinaIzdanja.ToString();
        public string ZanrDisplay => string.IsNullOrWhiteSpace(Zanr) ? "nepoznato" : Zanr;

        // Display authors' full names or "nepoznato" when list is empty
        public string AutoriDisplay
        {
            get
            {
                if (Autori == null || Autori.Count == 0) return "nepoznato";
                return string.Join(", ", Autori.Select(a => string.IsNullOrWhiteSpace(a?.Ime) && string.IsNullOrWhiteSpace(a?.Prezime) ? "nepoznato" : $"{a.Ime} {a.Prezime}"));
            }
        }

        public override string ToString()
            {
                return $"Knjiga: {Naziv} | ISBN: {ISBN} | Žanr: {Zanr} | Cena: {Cena} RSD";
            }
        }
    
}

