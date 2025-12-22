using System;

namespace Core.Models
{
        public class Knjiga
        {
        public string ISBN { get; set; }
        public string Naziv { get; set; }
        public string Zanr { get; set; }
        public int GodinaIzdanja { get; set; }
        public double Cena { get; set; }
        public int BrojStrana { get; set; }
        public List<Autor> Autori { get; set; } = new List<Autor>();
        public string Izdavac { get; set; }
        public List<Posetilac> PosetiociKupili { get; set; } = new List<Posetilac>();
        public List<Posetilac> PosetiociListaZelja { get; set; } = new List<Posetilac>();

        public Knjiga()
        {
            PosetiociKupili = new List<Posetilac>();
            PosetiociListaZelja = new List<Posetilac>();
        }
        public override string ToString()
            {
                return $"Knjiga: {Naziv} | ISBN: {ISBN} | Žanr: {Zanr} | Cena: {Cena} RSD";
            }
        }
    
}

