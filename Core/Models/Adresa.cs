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
    public class Adresa
    {
        public int Id { get; set; }
        public string Ulica { get; set; }
        public string Broj { get; set; }
        public string Grad { get; set; }
        public string Drzava { get; set; }

        // Prazan konstruktor
        public Adresa() { }  
 
        // Konstruktor za lakši unos
        public Adresa(int id, string ulica, string broj, string grad, string drzava)
        {
            Id = id;
            Ulica = ulica;
            Broj = broj;
            Grad = grad;
            Drzava = drzava;
        }

        public override string ToString()
        {
            // Vraća formatiran string: Ulica Broj, Grad, Drzava
            string d = string.IsNullOrWhiteSpace(Drzava) ? "" : $", {Drzava}";
            return $"{Ulica} {Broj}, {Grad}{d}";
        }

    }
}