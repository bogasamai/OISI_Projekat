using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SajamKnjiga.Models
{
    public class Kupovina
    {
        public Posetilac Posetilac { get; set; }
        public Knjiga Knjiga { get; set; }

        public DateTime DatumKupovine { get; set; }
        public int Ocena { get; set; } 
        public string Komentar { get; set; }

        public Kupovina() { }

        public override string ToString()
        {
            return $"Kupovina: {Knjiga?.Naziv} | Kupac: {Posetilac?.Ime} {Posetilac?.Prezime} | " +
                   $"Datum: {DatumKupovine.ToShortDateString()} | Ocena: {Ocena}";
        }

    }
}
