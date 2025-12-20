using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SajamKnjiga.Models
{
    public enum StatusPosetioca
    {
        R,
        V  
    }

    internal class Posetilac
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string Adresa { get; set; }
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

    }
}
