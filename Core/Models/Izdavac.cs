using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Izdavac
    {
        public string Sifra { get; set; }
        public string Naziv { get; set; }

        public Autor SefIzdavaca { get; set; }

        public List<Autor> SpisakAutora { get; set; } = new List<Autor>();
        public List<Knjiga> SpisakKnjiga { get; set; } = new List<Knjiga>();
        
        public Izdavac() { }

        public override string ToString()
        {
            return $"Izdavač: {Naziv} (Šifra: {Sifra}) | Šef: {SefIzdavaca?.Ime} {SefIzdavaca?.Prezime}";
        }


    }
}
