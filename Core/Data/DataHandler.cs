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
        private static string putanjaAutorKnjiga = Path.Combine(folder, "autor_knjiga.txt");

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
                    sw.WriteLine($"{a.Ime}|{a.Prezime}|{a.BrojLicneKarte}|{a.Email}|{a.GodineIskustva}|{a.Telefon}|{a.DatumRodjenja:yyyy-MM-dd}");
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

                var autor = new Autor
                {
                    Ime = d[0],
                    Prezime = d[1],
                    BrojLicneKarte = d[2],
                    Email = d[3],
                    GodineIskustva = int.Parse(d[4]),
                    Telefon = d[5]
                };

                // Dodaj DatumRodjenja ako postoji u fajlu
                if (d.Length >= 7 && DateTime.TryParse(d[6], out DateTime datum))
                {
                    autor.DatumRodjenja = datum;
                }

                rezultat.Add(autor);
            }
            return rezultat;
        }

        // --- VEZE AUTOR-KNJIGA ---
        public static void SacuvajVezeAutorKnjiga(List<Autor> autori)
        {
            ProveriFolder();
            using (StreamWriter sw = new StreamWriter(putanjaAutorKnjiga))
            {
                foreach (var a in autori)
                {
                    if (a.SpisakKnjiga == null) continue;
                    foreach (var k in a.SpisakKnjiga)
                    {
                        sw.WriteLine($"{a.BrojLicneKarte}|{k.ISBN}");
                    }
                }
            }
        }

        public static void PoveziAutoreIKnjige(List<Autor> autori, List<Knjiga> knjige)
        {
            if (!File.Exists(putanjaAutorKnjiga)) return;

            foreach (string linija in File.ReadAllLines(putanjaAutorKnjiga))
            {
                string[] d = linija.Split('|');
                if (d.Length < 2) continue;

                var autor = autori.FirstOrDefault(a => a.BrojLicneKarte == d[0]);
                var knjiga = knjige.FirstOrDefault(k => k.ISBN == d[1]);

                if (autor != null && knjiga != null)
                {
                    if (autor.SpisakKnjiga != null && !autor.SpisakKnjiga.Any(k => k.ISBN == knjiga.ISBN))
                        autor.SpisakKnjiga.Add(knjiga);
                    if (knjiga.Autori != null && !knjiga.Autori.Any(a => a.BrojLicneKarte == autor.BrojLicneKarte))
                        knjiga.Autori.Add(autor);
                }
            }
        }

        // --- KNJIGE ---
        public static void SacuvajKnjige(List<Knjiga> knjige)
        {
            ProveriFolder();
            using (StreamWriter sw = new StreamWriter(putanjaKnjige))
            {
                foreach (var k in knjige)
                {
                    sw.WriteLine($"{k.ISBN}|{k.Naziv}|{k.Zanr}|{k.Cena}|{k.GodinaIzdanja}|{k.Izdavac}|{k.BrojStrana}");
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
                    Izdavac = d[5],
                    BrojStrana = int.TryParse(d[6], out int brojStrana) ? brojStrana : 0

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
                    // Moramo "izvući" polja iz objekta Adresa. 
                    // Koristimo ?. operator u slučaju da je Adresa null da program ne pukne.
                    string ulica = p.Adresa?.Ulica ?? "Nepoznato";
                    string broj = p.Adresa?.Broj ?? "/";
                    string grad = p.Adresa?.Grad ?? "Nepoznato";
                    string drzava = p.Adresa?.Drzava ?? "";

                    // Dodajemo adresa polja na kraj linije (indeksi 5, 6, 7 i 8)
                    sw.WriteLine($"{p.Ime}|{p.Prezime}|{p.BrojClanskeKarte}|{p.Status}|{p.Email}|{ulica}|{broj}|{grad}|{drzava}");
                }
            }
        }

        public static List<Posetilac> UcitajPosetioce()
        {
            List<Posetilac> rezultat = new List<Posetilac>();
            if (!File.Exists(putanjaPosetioci)) return rezultat;

            foreach (string linija in File.ReadAllLines(putanjaPosetioci))
            {
                if (string.IsNullOrWhiteSpace(linija)) continue;

                string[] d = linija.Split('|');

                // Osnovni podaci
                Posetilac p = new Posetilac
                {
                    Ime = d[0],
                    Prezime = d[1],
                    BrojClanskeKarte = d[2],
                    Status = (StatusPosetioca)Enum.Parse(typeof(StatusPosetioca), d[3]),
                    Email = d[4]
                };

                // Ako linija ima bar 9 delova, znači da imamo i adresu
                if (d.Length >= 9)
                {
                    p.Adresa = new Adresa
                    {
                        Id = 0, // Id možemo ostaviti na 0 ili generisati
                        Ulica = d[5],
                        Broj = d[6],
                        Grad = d[7],
                        Drzava = d[8]
                    };
                }

                rezultat.Add(p);
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