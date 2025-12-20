using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SajamKnjiga.Models
{
    internal class Kupovina
    {
        // Reference ka objektima (koristimo ? jer u Domaćem 1 veze ne moraju biti pune)
        public Posetilac Posetilac { get; set; }
        public Knjiga Knjiga { get; set; }

        public DateTime DatumKupovine { get; set; }
        public int Ocena { get; set; } // 1-5
        public string Komentar { get; set; }

        public Kupovina() { }

        public override string ToString()
        {
            return $"Kupovina: {Knjiga?.Naziv} | Kupac: {Posetilac?.Ime} {Posetilac?.Prezime} | " +
                   $"Datum: {DatumKupovine.ToShortDateString()} | Ocena: {Ocena}";
        }

    }
}
