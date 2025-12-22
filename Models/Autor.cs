using System;

namespace Core.Models
{
   public class Autor
{
    public string Ime { get; set; } //IM
    public string Prezime { get; set; } //PR
    public DateTime DatumRodjenja { get; set; } //DR
    public Adresa AdresaStanovanja { get; set; } 
    public string Telefon { get; set; } // TEL
    public string Email { get; set; }
    public string BrojLicneKarte { get; set; } //LK
    public int GodineIskustva { get; set; }
    public List<Knjiga> SpisakKnjiga { get; set; } 

    public Autor()
    {
        SpisakKnjiga = new List<Knjiga>();
    }

        public override string ToString()
        {
            return $"Autor: {Ime} {Prezime} | LK: {BrojLicneKarte} | Email: {Email} | Iskustvo: {GodineIskustva} god.";
        }

    }
}