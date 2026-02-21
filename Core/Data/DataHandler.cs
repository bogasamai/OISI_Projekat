using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Data
{
    public class DataHandler
    {
        private static string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static string folder = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "Core", "podaci"));
        private static string putanjaPosetioci = Path.Combine(folder, "posetioci.txt");
        private static string putanjaIzdavaci = Path.Combine(folder, "izdavaci.txt");
        private static string putanjaKupovine = Path.Combine(folder, "kupovine.txt");
        private static string putanjaKnjige = Path.Combine(folder, "knjige.txt");
        private static string putanjaAutori = Path.Combine(folder, "autori.txt");

        private static void ProveriFolder()
        {
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            Console.WriteLine(putanjaKnjige);

        }

        // --- AUTORI ---
        public static void SacuvajAutore(List<Autor> autori)
        {
            ProveriFolder();
            using (StreamWriter sw = new StreamWriter(putanjaAutori))
            {
                foreach (var a in autori)
                {
                    sw.WriteLine($"{a.Ime}|{a.Prezime}|{a.BrojLicneKarte}|{a.Email}|{a.GodineIskustva}|{a.Telefon}");
                }
            }
        }

        public static List<Autor> UcitajAutore()
        {
            List<Autor> rezultat = new List<Autor>();
            if (!File.Exists(putanjaAutori)) return rezultat;

            foreach (string linija in File.ReadAllLines(putanjaAutori))
            {
                string[] d = linija.Split('|');
                if (d.Length < 6) continue;
                rezultat.Add(new Autor
                {
                    Ime = d[0],
                    Prezime = d[1],
                    BrojLicneKarte = d[2],
                    Email = d[3],
                    GodineIskustva = int.Parse(d[4]),
                    Telefon = d[5]
                });
            }
            return rezultat;
        }

        // --- KNJIGE ---
        public static void SacuvajKnjige(List<Knjiga> knjige)
        {
            ProveriFolder();
            using (StreamWriter sw = new StreamWriter(putanjaKnjige))
            {
                foreach (var k in knjige)
                {
                    sw.WriteLine($"{k.ISBN}|{k.Naziv}|{k.Zanr}|{k.Cena}|{k.GodinaIzdanja}|{k.Izdavac}");
                }
            }
        }

        public static List<Knjiga> UcitajKnjige()
        {
            List<Knjiga> rezultat = new List<Knjiga>();
            if (!File.Exists(putanjaKnjige)) return rezultat;

            foreach (string linija in File.ReadAllLines(putanjaKnjige))
            {
                string[] d = linija.Split('|');
                if (d.Length < 6) continue;
                rezultat.Add(new Knjiga
                {
                    ISBN = d[0],
                    Naziv = d[1],
                    Zanr = d[2],
                    Cena = double.Parse(d[3]),
                    GodinaIzdanja = int.Parse(d[4]),
                    Izdavac = d[5]
                });
            }
            return rezultat;
        }

        // --- POSETIOCI ---
        public static void SacuvajPosetioce(List<Posetilac> posetioci)
        {
            ProveriFolder();
            using (StreamWriter sw = new StreamWriter(putanjaPosetioci))
            {
                foreach (var p in posetioci)
                {
                    sw.WriteLine($"{p.Ime}|{p.Prezime}|{p.BrojClanskeKarte}|{p.Status}|{p.Email}");
                }
            }
        }

        public static List<Posetilac> UcitajPosetioce()
        {
            List<Posetilac> rezultat = new List<Posetilac>();
            if (!File.Exists(putanjaPosetioci)) return rezultat;

            foreach (string linija in File.ReadAllLines(putanjaPosetioci))
            {
                string[] d = linija.Split('|');
                rezultat.Add(new Posetilac
                {
                    Ime = d[0],
                    Prezime = d[1],
                    BrojClanskeKarte = d[2],
                    Status = (StatusPosetioca)Enum.Parse(typeof(StatusPosetioca), d[3]),
                    Email = d[4]
                });
            }
            return rezultat;
        }

        // --- IZDAVAČI ---
        public static void SacuvajIzdavace(List<Izdavac> izdavaci)
        {
            ProveriFolder();
            using (StreamWriter sw = new StreamWriter(putanjaIzdavaci))
            {
                foreach (var i in izdavaci)
                {
                    sw.WriteLine($"{i.Sifra}|{i.Naziv}|{i.SefIzdavaca?.Ime}|{i.SefIzdavaca?.Prezime}|{i.SefIzdavaca?.GodineIskustva}");
                }
            }
        }

        public static List<Izdavac> UcitajIzdavace()
        {
            List<Izdavac> rezultat = new List<Izdavac>();
            if (!File.Exists(putanjaIzdavaci)) return rezultat;

            foreach (string linija in File.ReadAllLines(putanjaIzdavaci))
            {
                string[] d = linija.Split('|');
                rezultat.Add(new Izdavac
                {
                    Sifra = d[0],
                    Naziv = d[1],
                    SefIzdavaca = new Autor { Ime = d[2], Prezime = d[3], GodineIskustva = int.Parse(d[4]) }
                });
            }
            return rezultat;
        }
    }
}