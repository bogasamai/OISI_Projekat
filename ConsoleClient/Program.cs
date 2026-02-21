using Core.Data;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Učitavanje svih podataka na startu
            List<Posetilac> sviPosetioci = DataHandler.UcitajPosetioce();
            List<Izdavac> sviIzdavaci = DataHandler.UcitajIzdavace();
            List<Autor> sviAutori = DataHandler.UcitajAutore();
            List<Knjiga> sveKnjige = DataHandler.UcitajKnjige();

            bool kraj = false;
            while (!kraj)
            {
                Console.WriteLine("\n===== SAJAM KNJIGA - GLAVNI MENI =====");
                Console.WriteLine("1. Upravljanje Posetiocima");
                Console.WriteLine("2. Upravljanje Izdavačima");
                Console.WriteLine("3. Upravljanje Autorima");
                Console.WriteLine("4. Upravljanje Knjigama");
                Console.WriteLine("5. Sačuvaj i izađi");
                Console.Write("Izbor: ");

                string izbor = Console.ReadLine();

                switch (izbor)
                {
                    case "1":
                        MeniPosetioci(sviPosetioci);
                        break;
                    case "2":
                        MeniIzdavaci(sviIzdavaci);
                        break;
                    case "3":
                        MeniAutori(sviAutori);
                        break;
                    case "4":
                        MeniKnjige(sveKnjige);
                        break;
                    case "5":
                        // 2. Čuvanje apsolutno svega pre gašenja
                        DataHandler.SacuvajPosetioce(sviPosetioci);
                        DataHandler.SacuvajIzdavace(sviIzdavaci);
                        DataHandler.SacuvajAutore(sviAutori);
                        DataHandler.SacuvajKnjige(sveKnjige);
                        Console.WriteLine("Svi podaci su uspešno sačuvani u folder 'podaci'. Pozdrav!");
                        kraj = true;
                        break;
                    default:
                        Console.WriteLine("Nepostojeća opcija.");
                        break;
                }
            }
        }

        // --- POMOĆNE METODE ZA MENIJE ---

        static void MeniPosetioci(List<Posetilac> posetioci)
        {
            Console.WriteLine("\n--- Posetioci ---");
            Console.WriteLine("1. Prikaz svih");
            Console.WriteLine("2. Unos novog");
            string izb = Console.ReadLine();
            if (izb == "1") posetioci.ForEach(p => Console.WriteLine(p));
            else if (izb == "2")
            {
                Console.Write("Ime: "); string i = Console.ReadLine();
                Console.Write("Prezime: "); string p = Console.ReadLine();
                Console.Write("Broj članske: "); string b = Console.ReadLine();
                posetioci.Add(new Posetilac { Ime = i, Prezime = p, BrojClanskeKarte = b, Status = StatusPosetioca.R });
                Console.WriteLine("Posetilac dodat.");
            }
        }

        static void MeniAutori(List<Autor> autori)
        {
            Console.WriteLine("\n--- Autori ---");
            Console.WriteLine("1. Prikaz svih");
            Console.WriteLine("2. Unos novog");
            string izb = Console.ReadLine();
            if (izb == "1") autori.ForEach(a => Console.WriteLine(a));
            else if (izb == "2")
            {
                Console.Write("Ime: "); string i = Console.ReadLine();
                Console.Write("Prezime: "); string p = Console.ReadLine();
                Console.Write("LK: "); string lk = Console.ReadLine();
                Console.Write("Iskustvo (god): "); int god = int.Parse(Console.ReadLine());
                autori.Add(new Autor { Ime = i, Prezime = p, BrojLicneKarte = lk, GodineIskustva = god });
                Console.WriteLine("Autor dodat.");
            }
        }

        static void MeniKnjige(List<Knjiga> knjige)
        {
            Console.WriteLine("\n--- Knjige ---");
            Console.WriteLine("1. Prikaz svih");
            Console.WriteLine("2. Unos nove");
            string izb = Console.ReadLine();
            if (izb == "1") knjige.ForEach(k => Console.WriteLine(k));
            else if (izb == "2")
            {
                Console.Write("Naziv: "); string n = Console.ReadLine();
                Console.Write("ISBN: "); string isbn = Console.ReadLine();
                Console.Write("Cena: "); double c = double.Parse(Console.ReadLine());
                knjige.Add(new Knjiga { Naziv = n, ISBN = isbn, Cena = c });
                Console.WriteLine("Knjiga dodata.");
            }
        }

        static void MeniIzdavaci(List<Izdavac> izdavaci)
        {
            Console.WriteLine("\n--- Izdavači ---");
            Console.WriteLine("1. Prikaz svih");
            Console.WriteLine("2. Unos novog (min 5 god iskustva šefa)");
            string izb = Console.ReadLine();
            if (izb == "1") izdavaci.ForEach(i => Console.WriteLine(i));
            else if (izb == "2")
            {
                Console.Write("Naziv: "); string naz = Console.ReadLine();
                Console.Write("Šifra: "); string sif = Console.ReadLine();
                Console.Write("Ime šefa: "); string im = Console.ReadLine();
                Console.Write("Iskustvo šefa: "); int g = int.Parse(Console.ReadLine());

                if (g >= 5)
                {
                    izdavaci.Add(new Izdavac { Naziv = naz, Sifra = sif, SefIzdavaca = new Autor { Ime = im, GodineIskustva = g } });
                    Console.WriteLine("Izdavač dodat.");
                }
                else
                {
                    Console.WriteLine("Greška: Šef nema dovoljno iskustva.");
                }
            }
        }
    }
}