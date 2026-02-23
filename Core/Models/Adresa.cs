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
            // If user stored full address in Ulica (free text), return it
            if (!string.IsNullOrWhiteSpace(Ulica) && string.IsNullOrWhiteSpace(Broj) && string.IsNullOrWhiteSpace(Grad) && string.IsNullOrWhiteSpace(Drzava))
                return Ulica.Trim();

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Ulica))
            {
                var addr = Ulica.Trim();
                if (!string.IsNullOrWhiteSpace(Broj)) addr += " " + Broj.Trim();
                parts.Add(addr);
            }
            if (!string.IsNullOrWhiteSpace(Grad)) parts.Add(Grad.Trim());
            if (!string.IsNullOrWhiteSpace(Drzava)) parts.Add(Drzava.Trim());

            return parts.Count == 0 ? string.Empty : string.Join(", ", parts);
        }
    }
}