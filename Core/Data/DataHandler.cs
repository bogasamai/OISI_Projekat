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
        }

        // --- AUTORI ---
        public static void SacuvajAutore(List<Autor> autori)
        {
            ProveriFolder();
            using (StreamWriter sw = new StreamWriter(putanjaAutori))
            {
                foreach (var a in autori)
                {
                    // Adresa se cuva u istom formatu kao kod posetioca: Ulica,Broj,Grad,Drzava
                    string adresaField = "";
                    if (a.AdresaStanovanja != null)
                    {
                        string ulica = a.AdresaStanovanja.Ulica ?? "";
                        string broj = a.AdresaStanovanja.Broj ?? "";
                        string grad = a.AdresaStanovanja.Grad ?? "";
                        string drzava = a.AdresaStanovanja.Drzava ?? "";
                        adresaField = $"{ulica},{broj},{grad},{drzava}";
                    }

                    sw.WriteLine($"{a.Ime}|{a.Prezime}|{a.BrojLicneKarte}|{a.Email}|{a.GodineIskustva}|{a.Telefon}|{a.DatumRodjenja:yyyy-MM-dd}|{adresaField}");
                }
            }
        }

        public static List<Autor> UcitajAutore()
        {
            List<Autor> rezultat = new List<Autor>();
            if (!File.Exists(putanjaAutori)) return rezultat;

            foreach (string linija in File.ReadAllLines(putanjaAutori))
            {
                if (string.IsNullOrWhiteSpace(linija)) continue;
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

                // Dodaj Adresu ako postoji u fajlu (indeks 7 = "Ulica,Broj,Grad,Drzava")
                if (d.Length >= 8 && !string.IsNullOrWhiteSpace(d[7]))
                {
                    string[] adrDelovi = d[7].Split(',');
                    autor.AdresaStanovanja = new Adresa
                    {
                        Ulica = adrDelovi.Length > 0 ? adrDelovi[0].Trim() : "",
                        Broj = adrDelovi.Length > 1 ? adrDelovi[1].Trim() : "",
                        Grad = adrDelovi.Length > 2 ? adrDelovi[2].Trim() : "",
                        Drzava = adrDelovi.Length > 3 ? adrDelovi[3].Trim() : ""
                    };
                }

                rezultat.Add(autor);
            }
            return rezultat;
        }

        // Update single posetilac in storage (loads existing file, replaces matching by BrojClanskeKarte, saves back)
        public static void UpdatePosetilac(Posetilac posetilac)
        {
            var svi = UcitajPosetioce();
            var existing = svi.FirstOrDefault(p => p.BrojClanskeKarte == posetilac.BrojClanskeKarte);
            if (existing != null)
            {
                int idx = svi.IndexOf(existing);
                svi[idx] = posetilac;
            }
            else
            {
                svi.Add(posetilac);
            }
            // Save posetioci and purchases to keep kupovine in sync
            SacuvajPosetioce(svi);
            SacuvajKupovine(svi);
        }

        // --- KNJIGE ---
        public static void SacuvajKnjige(List<Knjiga> knjige)
        {
            ProveriFolder();
            using (StreamWriter sw = new StreamWriter(putanjaKnjige))
            {
                foreach (var k in knjige)
                {
                    // Serialize authors by their BrojLicneKarte (comma-separated), if any
                    string authorsField = "";
                    if (k.Autori != null && k.Autori.Count > 0)
                    {
                        authorsField = string.Join(",", k.Autori.Where(a => a != null && !string.IsNullOrWhiteSpace(a.BrojLicneKarte)).Select(a => a.BrojLicneKarte));
                    }

                    sw.WriteLine($"{k.ISBN}|{k.Naziv}|{k.Zanr}|{k.Cena}|{k.GodinaIzdanja}|{k.Izdavac}|{k.BrojStrana}|{authorsField}");
                }
            }
        }

        public static List<Knjiga> UcitajKnjige()
        {
            List<Knjiga> rezultat = new List<Knjiga>();
            if (!File.Exists(putanjaKnjige)) return rezultat;
            var sviAutori = UcitajAutore();

            foreach (string linija in File.ReadAllLines(putanjaKnjige))
            {
                if (string.IsNullOrWhiteSpace(linija)) continue;
                string[] d = linija.Split('|');
                if (d.Length < 7) continue;

                var knjiga = new Knjiga
                {
                    ISBN = d[0],
                    Naziv = d[1],
                    Zanr = d[2],
                    Cena = double.TryParse(d[3], out double cena) ? cena : 0,
                    GodinaIzdanja = int.TryParse(d[4], out int godina) ? godina : 0,
                    Izdavac = d[5],
                    BrojStrana = int.TryParse(d[6], out int brojStrana) ? brojStrana : 0
                };

                if (d.Length >= 8 && !string.IsNullOrWhiteSpace(d[7]))
                {
                    var authIds = d[7].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                    foreach (var id in authIds)
                    {
                        var match = sviAutori.FirstOrDefault(a => a.BrojLicneKarte == id);
                        if (match != null) knjiga.Autori.Add(match);
                    }
                }

                rezultat.Add(knjiga);
            }

            // Rekonstruisi SpisakKnjiga za svakog autora na osnovu veza iz knjiga
            foreach (var autor in sviAutori)
            {
                autor.SpisakKnjiga = rezultat.Where(k => k.Autori.Any(a => a.BrojLicneKarte == autor.BrojLicneKarte)).ToList();
            }

            return rezultat;
        }

        /// <summary>
        /// Ucitava sve podatke sinhronizovano - autori, knjige, posetioci, kupovine.
        /// Ovo osigurava da su bidirekcione veze ispravno postavljene.
        /// </summary>
        public static (List<Autor> autori, List<Knjiga> knjige, List<Posetilac> posetioci) UcitajSve()
        {
            var autori = UcitajAutore();

            // Ucitaj knjige i povezuj autore
            List<Knjiga> knjige = new List<Knjiga>();
            if (File.Exists(putanjaKnjige))
            {
                foreach (string linija in File.ReadAllLines(putanjaKnjige))
                {
                    if (string.IsNullOrWhiteSpace(linija)) continue;
                    string[] d = linija.Split('|');
                    if (d.Length < 7) continue;

                    var knjiga = new Knjiga
                    {
                        ISBN = d[0],
                        Naziv = d[1],
                        Zanr = d[2],
                        Cena = double.TryParse(d[3], out double cena) ? cena : 0,
                        GodinaIzdanja = int.TryParse(d[4], out int godina) ? godina : 0,
                        Izdavac = d[5],
                        BrojStrana = int.TryParse(d[6], out int brojStrana) ? brojStrana : 0
                    };

                    if (d.Length >= 8 && !string.IsNullOrWhiteSpace(d[7]))
                    {
                        var authIds = d[7].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                        foreach (var id in authIds)
                        {
                            var match = autori.FirstOrDefault(a => a.BrojLicneKarte == id);
                            if (match != null) knjiga.Autori.Add(match);
                        }
                    }

                    knjige.Add(knjiga);
                }
            }

            // Rekonstruisi SpisakKnjiga za svakog autora
            foreach (var autor in autori)
            {
                autor.SpisakKnjiga = knjige.Where(k => k.Autori.Any(a => a.BrojLicneKarte == autor.BrojLicneKarte)).ToList();
            }

            // Ucitaj posetioce
            var posetioci = UcitajPosetioce();

            // Ucitaj kupovine i povezi sa posetiocima i knjigama
            if (File.Exists(putanjaKupovine))
            {
                foreach (string linija in File.ReadAllLines(putanjaKupovine))
                {
                    if (string.IsNullOrWhiteSpace(linija)) continue;
                    string[] d = linija.Split('|');
                    if (d.Length < 5) continue;

                    string brojKarte = d[0];
                    string isbn = d[1];
                    DateTime datumKupovine = DateTime.TryParse(d[2], out DateTime dt) ? dt : DateTime.Now;
                    int ocena = int.TryParse(d[3], out int oc) ? oc : 0;
                    string komentar = d[4];

                    var posetilac = posetioci.FirstOrDefault(p => p.BrojClanskeKarte == brojKarte);
                    var knjiga = knjige.FirstOrDefault(k => k.ISBN == isbn);

                    if (posetilac != null && knjiga != null)
                    {
                        var kupovina = new Kupovina
                        {
                            Posetilac = posetilac,
                            Knjiga = knjiga,
                            DatumKupovine = datumKupovine,
                            Ocena = ocena,
                            Komentar = komentar
                        };
                        posetilac.KupljeneKnjige.Add(kupovina);
                        knjiga.PosetiociKupili.Add(posetilac);
                    }
                }
            }

            // Map wishlist placeholders (Knjiga objects with only ISBN) to actual Knjiga instances
            foreach (var pos in posetioci)
            {
                if (pos.ListaZelja == null || pos.ListaZelja.Count == 0) continue;
                var mapped = new List<Knjiga>();
                foreach (var wish in pos.ListaZelja)
                {
                    if (wish == null || string.IsNullOrWhiteSpace(wish.ISBN)) continue;
                    var match = knjige.FirstOrDefault(k => k.ISBN == wish.ISBN);
                    if (match != null) mapped.Add(match);
                }
                pos.ListaZelja = mapped;
            }

            return (autori, knjige, posetioci);
        }

        /// <summary>
        /// Cuva sve podatke - autore, knjige, posetioce i kupovine.
        /// </summary>
        public static void SacuvajSve(List<Autor> autori, List<Knjiga> knjige, List<Posetilac> posetioci)
        {
            SacuvajAutore(autori);
            SacuvajKnjige(knjige);
            SacuvajPosetioce(posetioci);
            SacuvajKupovine(posetioci);
        }

        // --- KUPOVINE ---
        public static void SacuvajKupovine(List<Posetilac> posetioci)
        {
            ProveriFolder();
            using (StreamWriter sw = new StreamWriter(putanjaKupovine))
            {
                foreach (var p in posetioci)
                {
                    if (p.KupljeneKnjige == null) continue;
                    foreach (var kup in p.KupljeneKnjige)
                    {
                        if (kup.Knjiga == null) continue;
                        sw.WriteLine($"{p.BrojClanskeKarte}|{kup.Knjiga.ISBN}|{kup.DatumKupovine:yyyy-MM-dd}|{kup.Ocena}|{kup.Komentar ?? ""}");
                    }
                }
            }
        }

        // --- POSETIOCI ---
        public static void SacuvajPosetioce(List<Posetilac> posetioci)
        {
            ProveriFolder();
            var linije = new List<string>();

            foreach (var p in posetioci)
            {
                // Datum cuvamo u ISO 8601 ("o") formatu radi pouzdanog parsiranja
                string datum = p.DatumRodjenja.ToString("o");

                string ulica = p.Adresa?.Ulica ?? "Nepoznato";
                string broj = p.Adresa?.Broj ?? "/";
                string grad = p.Adresa?.Grad ?? "Nepoznato";
                string drzava = p.Adresa?.Drzava ?? "";
                string telefon = p.Telefon ?? "";

                // Format (novo): Ime|Prezime|BrojClanske|Status|Email|Datum|Ulica|Broj|Grad|Drzava|Telefon
                // serialize wishlist as comma-separated ISBNs
                string wishlistField = "";
                if (p.ListaZelja != null && p.ListaZelja.Count > 0)
                {
                    wishlistField = string.Join(",", p.ListaZelja.Where(k => k != null && !string.IsNullOrWhiteSpace(k.ISBN)).Select(k => k.ISBN));
                }

                string linija = $"{p.Ime}|{p.Prezime}|{p.BrojClanskeKarte}|{p.Status}|{p.Email}|{datum}|{ulica}|{broj}|{grad}|{drzava}|{telefon}|{wishlistField}";
                linije.Add(linija);
            }

            File.WriteAllLines(putanjaPosetioci, linije);
        }

        public static List<Posetilac> UcitajPosetioce()
        {
            List<Posetilac> rezultat = new List<Posetilac>();
            if (!File.Exists(putanjaPosetioci)) return rezultat;

            foreach (string linija in File.ReadAllLines(putanjaPosetioci))
            {
                if (string.IsNullOrWhiteSpace(linija)) continue;

                string[] d = linija.Split('|');
                if (d.Length < 5) continue; // minimum polja: ime, prezime, brojKarte, status, email

                Posetilac p = new Posetilac
                {
                    Ime = d[0],
                    Prezime = d[1],
                    BrojClanskeKarte = d[2],
                    Status = (StatusPosetioca)Enum.Parse(typeof(StatusPosetioca), d[3]),
                    Email = d[4]
                };

                // Parsiranje datuma rodjenja:
                DateTime datumRodjenja = DateTime.Now;
                int addrStartIndex = 5; // podrazumevani start adrese (stari format: nakon email sledi ulica)
                if (d.Length > 5)
                {
                    // Pokusaj da parsiramo polje d[5] kao datum.
                    if (DateTime.TryParse(d[5], out DateTime parsedDate))
                    {
                        datumRodjenja = parsedDate;
                        addrStartIndex = 6; // adresa pocinje nakon datuma (novi format)
                    }
                    else
                    {
                        // nije datum -> tretiramo ga kao pocetak adrese (stari format)
                        datumRodjenja = DateTime.Now;
                        addrStartIndex = 5;
                    }
                }
                p.DatumRodjenja = datumRodjenja;

                // Adresa (ocekivano 4 polja: ulica, broj, grad, drzava)
                if (d.Length >= addrStartIndex + 4)
                {
                    p.Adresa = new Adresa
                    {
                        Id = 0,
                        Ulica = d[addrStartIndex],
                        Broj = d[addrStartIndex + 1],
                        Grad = d[addrStartIndex + 2],
                        Drzava = d[addrStartIndex + 3]
                    };
                }

                // Telefon: u novom formatu nalazi se posle adrese; u starom formatu telefon je možda prazan/izostavljen
                if (d.Length > addrStartIndex + 4)
                {
                    p.Telefon = d[addrStartIndex + 4];
                }
                else
                {
                    p.Telefon = ""; // fallback ako nema telefona u fajlu
                }

                // Wishlist: ako postoji polje nakon telefona, tretiraj ga kao comma-separated listu ISBN-ova
                if (d.Length > addrStartIndex + 5 && !string.IsNullOrWhiteSpace(d[addrStartIndex + 5]))
                {
                    var isbns = d[addrStartIndex + 5].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                    p.ListaZelja = new List<Knjiga>();
                    foreach (var isbn in isbns)
                    {
                        p.ListaZelja.Add(new Knjiga { ISBN = isbn });
                    }
                }

                // Ostaviti ostalu logiku (kupovine, lista zelja) nepromenjenu
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