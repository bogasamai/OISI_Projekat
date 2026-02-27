# 📚 Sajam Knjiga 2026

> Informacioni sistem za upravljanje podacima o sajmu knjiga — projektni zadatak iz predmeta **Osnove Informacionih Sistema i Inženjerstva (OISI)**.
---

## 📖 O projektu

Aplikacija **Sajam Knjiga** omogućava kompletno upravljanje podacima vezanim za sajam knjiga: evidenciju posetilaca, autora, knjiga, izdavača i kupovina. Projekat je realizovan kao .NET 8 rešenje sa tri projekta i dva klijentska interfejsa — konzolnim i grafičkim (WPF).

## 🛠️ Tehnologije

| Tehnologija | Verzija |
|---|---|
| .NET | 8.0 |
| C# | 12 |
| WPF | .NET 8 (Windows) |
| Visual Studio | 2022 (v17+) |

### Perzistencija podataka
- Svi podaci se čuvaju u **tekstualne fajlove** (`|` delimiter) unutar `Core/podaci/`
- Automatsko kreiranje foldera ukoliko ne postoji
- Format: pipe-delimited (`|`) za laku čitljivost i parsiranje



## 📊 Domenski model

```
Posetilac (R/V)  ──kupuje──▶  Knjiga  ◀──piše──  Autor
       │                         │                   │
       │                         │                   │
   KupljeneKnjige            Izdavač            SpisakKnjiga
   ListaZelja            (šef = Autor)
```

### Preduslovi
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (preporučeno) ili bilo koji .NET IDE
- Windows OS (za WPF klijent)


## 👥 Tim i raspodela rada
Projekat je realizovan u timu od dva člana. Funkcionalnosti su podeljene kako bi se osigurala ravnopravna zastupljenost oba studenta.

### 👤 Student 1 - Luka Avramović
* **Modeli podataka:** `Posetilac`, `Kupovina`, `Izdavač`.
* **UI Komponente:** Izrada glavnog Menu Bar-a i Status Bar-a.
* **Upravljanje posetiocima:** CRUD operacije, napredna pretraga i sortiranje.
* **Logika knjiga:** Dodavanje/izmena knjiga, praćenje kupljenih naslova, dodavanje na listu želja.
* **Veze:** Povezivanje knjiga sa autorima i postavljanje šefova izdavačkih kuća.

### 👤 Student 2 - Vukašin Petrović
* **Modeli podataka:** `Autor`, `Knjiga`, `Adresa`.
* **UI Komponente:** Izrada glavnog prozora (MainWindow) i Toolbar-a.
* **Upravljanje autorima:** Kompletne CRUD operacije, pretraga i sortiranje autora.
* **Logika knjiga:** Prikaz i brisanje knjiga, pretraga po kriterijumima.
* **Veze:** Upravljanje listama želja, realizacija kupovine, upravljanje više autora na jednoj knjizi.
* **Izveštaji:** Pregled autora i knjiga po određenom izdavaču.


## 🛠 Instalacija i pokretanje
1. Klonirajte repozitorijum:
   ```bash
   git clone https://github.com/bogasamai/OISI_Projekat

> ⚠️ **Napomena za konzolnu verziju:** Obavezno izaći kroz opciju **5 (Sačuvaj i izađi)** kako bi se podaci sačuvali!
