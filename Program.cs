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
                new Posetilac {
                    Ime = "Luka",
                    Prezime = "Avramovic",
                    BrojClanskeKarte = "123",
                    Status = StatusPosetioca.V,
                    Email = "luka@email.com"
                },
                new Posetilac {
                    Ime = "Marko",
                    Prezime = "Markovic",
                    BrojClanskeKarte = "456",
                    Status = StatusPosetioca.R,
                    Email = "marko@email.com"
                }
            };

            // 2. Testiramo ČUVANJE
            Console.WriteLine("Čuvam posetioce u fajl...");
            DataHandler.SacuvajPosetioce(testPosetioci);
            Console.WriteLine("Čuvanje završeno.");

            // 3. Testiramo UČITAVANJE
            Console.WriteLine("\nUčitavam posetioce iz fajla...");
            List<Posetilac> ucitaniPosetioci = DataHandler.UcitajPosetioce();

            // 4. Ispisujemo učitano da proverimo da li je sve tu
            foreach (var p in ucitaniPosetioci)
            {
                Console.WriteLine(p.ToString());
            }

            Console.WriteLine("\nTest završen. Pritisni bilo koji taster za kraj.");
            Console.ReadKey();
        }
    }
}
