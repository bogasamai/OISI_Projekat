using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SajamKnjiga.Models;

namespace SajamKnjiga.Data
{
    internal class DataHandler
    {
     
        private static string putanjaPosetioci = "podaci" + Path.DirectorySeparatorChar + "posetioci.txt";

        public static void SacuvajPosetioce(List<Posetilac> posetioci)
        {
          
            string folder = "podaci";
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

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

            using (StreamReader sr = new StreamReader(putanjaPosetioci))
            {
                string linija;
                while ((linija = sr.ReadLine()) != null)
                {
                    string[] delovi = linija.Split('|');
                    Posetilac p = new Posetilac
                    {
                        Ime = delovi[0],
                        Prezime = delovi[1],
                        BrojClanskeKarte = delovi[2],
                        Status = (StatusPosetioca)Enum.Parse(typeof(StatusPosetioca), delovi[3]),
                        Email = delovi[4]
                    };
                    rezultat.Add(p);
                }
            }
            return rezultat;
        }
        private static string putanjaIzdavaci = "podaci" + Path.DirectorySeparatorChar + "izdavaci.txt";

        public static void SacuvajIzdavace(List<Izdavac> izdavaci)
        {
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

            using (StreamReader sr = new StreamReader(putanjaIzdavaci))
            {
                string linija;
                while ((linija = sr.ReadLine()) != null)
                {
                    string[] delovi = linija.Split('|');
                    Izdavac i = new Izdavac
                    {
                        Sifra = delovi[0],
                        Naziv = delovi[1],
                        SefIzdavaca = new Autor { Ime = delovi[2], Prezime = delovi[3], GodineIskustva = int.Parse(delovi[4]) }
                    };
                    rezultat.Add(i);
                }
            }
            return rezultat;
        }

        private static string putanjaKupovine = "podaci" + Path.DirectorySeparatorChar + "kupovine.txt";

        public static void SacuvajKupovine(List<Kupovina> kupovine)
        {
            using (StreamWriter sw = new StreamWriter(putanjaKupovine))
            {
                foreach (var k in kupovine)
                {
                    sw.WriteLine($"{k.Posetilac?.Ime}|{k.Knjiga?.Naziv}|{k.DatumKupovine}|{k.Ocena}");
                }
            }
        }


    }
}
