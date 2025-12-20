using SajamKnjiga.Data;
using SajamKnjiga.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SajamKnjiga
{
    internal class Program
    {
        static void Main(string[] args)
        {
          
            List<Posetilac> testPosetioci = new List<Posetilac>()
    {
        new Posetilac { Ime = "Luka", Prezime = "Avramovic", BrojClanskeKarte = "123", Status = StatusPosetioca.V, Email = "luka@email.com" },
        new Posetilac { Ime = "Marko", Prezime = "Markovic", BrojClanskeKarte = "456", Status = StatusPosetioca.R, Email = "marko@email.com" }
    };
            DataHandler.SacuvajPosetioce(testPosetioci);

         
            Console.WriteLine("\n--- Testiranje Izdavača ---");

            Autor iskusniAutor = new Autor { Ime = "Ivo", Prezime = "Andric", GodineIskustva = 10 };
            Autor mladiAutor = new Autor { Ime = "Pera", Prezime = "Peric", GodineIskustva = 2 };

            List<Izdavac> testIzdavaci = new List<Izdavac>();

         
            if (iskusniAutor.GodineIskustva >= 5)
            {
                testIzdavaci.Add(new Izdavac { Sifra = "IZD001", Naziv = "Laguna", SefIzdavaca = iskusniAutor });
                Console.WriteLine("Izdavač dodat uspešno (Šef ima dovoljno iskustva).");
            }
            else
            {
                Console.WriteLine("GREŠKA: Šef mora imati barem 5 godina iskustva!");
            }

            DataHandler.SacuvajIzdavace(testIzdavaci);

      
            Console.WriteLine("\n--- Testiranje Kupovine ---");
            Knjiga k1 = new Knjiga { Naziv = "Na Drini cuprija" };

            List<Kupovina> testKupovine = new List<Kupovina>()
    {
        new Kupovina {
            Posetilac = testPosetioci[0],
            Knjiga = k1,
            DatumKupovine = DateTime.Now,
            Ocena = 5
        }
    };
            DataHandler.SacuvajKupovine(testKupovine);

     
            Console.WriteLine("\n=== UCITANI PODACI IZ FAJLOVA ===");

            Console.WriteLine("\nPosetioci:");
            DataHandler.UcitajPosetioce().ForEach(p => Console.WriteLine(p));

            Console.WriteLine("\nIzdavači:");
            DataHandler.UcitajIzdavace().ForEach(i => Console.WriteLine(i));

            Console.WriteLine("\nTest završen. Pritisni bilo koji taster za kraj.");
            Console.ReadKey();
        }
    }
}
