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
            List<Posetilac> sviPosetioci = DataHandler.UcitajPosetioce();
            List<Izdavac> sviIzdavaci = DataHandler.UcitajIzdavace();

            bool kraj = false;
            while (!kraj)
            {
                Console.WriteLine("\n===== SAJAM KNJIGA - MENI =====");
                Console.WriteLine("1. Prikaz svih posetilaca");
                Console.WriteLine("2. Unos novog posetioca");
                Console.WriteLine("3. Unos novog izdavaca (PROVERA ŠEFA)");
                Console.WriteLine("4. Sačuvaj i izađi");
                Console.Write("Izbor: ");

                string izbor = Console.ReadLine();

                switch (izbor)
                {
                    case "1":
                        Console.WriteLine("\n--- Lista posetilaca ---");
                        sviPosetioci.ForEach(p => Console.WriteLine(p));
                        break;

                    case "2":
                        Console.Write("Ime: "); string ime = Console.ReadLine();
                        Console.Write("Prezime: "); string prezime = Console.ReadLine();
                        Console.Write("Email: "); string email = Console.ReadLine();
                        Console.Write("Broj članske karte: "); string bck = Console.ReadLine(); 
                        Console.Write("Telefon: "); string tel = Console.ReadLine();
                        Console.Write("Adresa: "); string adresa = Console.ReadLine();

                        sviPosetioci.Add(new Posetilac
                        {
                            Ime = ime,
                            Prezime = prezime,
                            Email = email,
                            BrojClanskeKarte = bck,
                            Telefon = tel,
                            Adresa = adresa,
                            Status = StatusPosetioca.R, 
                            GodinaClanstva = DateTime.Now.Year 
                        });
                        Console.WriteLine("Posetilac dodat u listu.");
                        break;

                    case "3":
                        Console.Write("Šifra izdavača: "); string sifraIzd = Console.ReadLine();
                        Console.Write("Naziv izdavača: "); string nazivIzd = Console.ReadLine();
                        Console.Write("Ime šefa: "); string imeSefa = Console.ReadLine();
                        Console.Write("Prezime šefa: "); string prezimeSefa = Console.ReadLine(); 
                        Console.Write("Godine iskustva šefa: ");
                        int iskustvo = int.Parse(Console.ReadLine());

                        if (iskustvo >= 5)
                        {
                            Autor sef = new Autor { Ime = imeSefa, Prezime = prezimeSefa, GodineIskustva = iskustvo };
                            sviIzdavaci.Add(new Izdavac { Sifra = sifraIzd, Naziv = nazivIzd, SefIzdavaca = sef });
                            Console.WriteLine("Izdavač uspešno dodat!");
                        }
                        else
                        {
                            Console.WriteLine("GREŠKA: Šef mora imati najmanje 5 godina iskustva!");
                        }
                        break;

                    case "4":
                        DataHandler.SacuvajPosetioce(sviPosetioci);
                        DataHandler.SacuvajIzdavace(sviIzdavaci);
                        Console.WriteLine("Podaci sačuvani. Pozdrav!");
                        kraj = true;
                        break;

                    default:
                        Console.WriteLine("Nepostojeća opcija.");
                        break;
                }
            }
        }
    }
}
