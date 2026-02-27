# 📚 Sajam Knjiga 2025

> Informacioni sistem za upravljanje podacima o sajmu knjiga — projektni zadatak iz predmeta **Osnove Informacionih Sistema i Inženjerstva (OISI)**.

## 📖 O projektu

Aplikacija **Sajam Knjiga** omogućava kompletno upravljanje podacima vezanim za sajam knjiga: evidenciju posetilaca, autora, knjiga, izdavača i kupovina. Projekat je realizovan kao .NET 8 rešenje sa tri projekta i dva klijentska interfejsa — konzolnim i grafičkim (WPF).

## 🏗️ Arhitektura

Rešenje (`SajamKnjiga.sln`) se sastoji od tri projekta:

```
SajamKnjiga.sln
├── Core/                  # Biblioteka sa modelima i pristupom podacima
│   ├── Models/            # Domenski modeli (entiteti)
│   │   ├── Posetilac.cs   # Posetilac sajma (Regularni / VIP)
│   │   ├── Autor.cs       # Autor knjige
│   │   ├── Knjiga.cs      # Knjiga sa ISBN, žanrom, cenom...
│   │   ├── Izdavac.cs     # Izdavačka kuća sa šefom i spiskom autora
│   │   ├── Kupovina.cs    # Kupovina (posetilac + knjiga + ocena)
│   │   └── Adresa.cs      # Adresa (ulica, broj, grad, država)
│   ├── Data/
│   │   └── DataHandler.cs  # Čitanje/pisanje podataka iz .txt fajlova
│   └── podaci/             # Tekstualni fajlovi sa podacima
│       ├── posetioci.txt
│       ├── autori.txt
│       ├── knjige.txt
│       ├── izdavaci.txt
│       └── kupovine.txt
│
├── ConsoleClient/          # Konzolna aplikacija (.NET 8)
│   └── Program.cs          # Tekstualni meni za CRUD operacije
│
└── WpfClient/              # WPF desktop aplikacija (.NET 8)
    ├── MainWindow.xaml      # Glavni prozor sa tabovima i pretragom
    ├── DodajPosetiocaWindow.xaml
    ├── DodajKnjiguWindow.xaml
    ├── IzmenaPosetiocaWindow.xaml
    ├── IzmenaKnjigeWindow.xaml
    ├── PonistiKupovinuDialog.xaml
    └── Resources/           # Ikonice (add, close)
```

## ✨ Funkcionalnosti

### Upravljanje entitetima
- **Posetioci** — dodavanje, izmena, prikaz, pretraga (Regularni / VIP status)
- **Autori** — dodavanje, izmena, prikaz, pretraga po imenu, prezimenu, broju LK
- **Knjige** — dodavanje, izmena, prikaz (ISBN, naziv, žanr, cena, broj strana, izdavač)
- **Izdavači** — evidencija izdavačkih kuća sa šefom i spiskom autora/knjiga

### Kupovine i statistike
- Evidentiranje kupovine knjiga od strane posetilaca
- Ocenjivanje i komentarisanje kupljenih knjiga
- Izračunavanje prosečne ocene i ukupne potrošnje po posetiocu
- Mogućnost poništavanja kupovine

### Dva klijentska interfejsa
| Konzolni (`ConsoleClient`) | Grafički (`WpfClient`) |
|---|---|
| Tekstualni meni sa 5 opcija | Moderni WPF interfejs sa tabovima |
| Brz unos/prikaz podataka | Pretraga u realnom vremenu |
| Čuvanje pri izlasku (opcija 5) | Automatsko čuvanje podataka |
| | Validacija formi pri unosu |
| | Prečice na tastaturi |
| | Status bar sa datumom i vremenom |

### Perzistencija podataka
- Svi podaci se čuvaju u **tekstualne fajlove** (`|` delimiter) unutar `Core/podaci/`
- Automatsko kreiranje foldera ukoliko ne postoji
- Format: pipe-delimited (`|`) za laku čitljivost i parsiranje

## 🛠️ Tehnologije

| Tehnologija | Verzija |
|---|---|
| .NET | 8.0 |
| C# | 12 |
| WPF | .NET 8 (Windows) |
| Visual Studio | 2022 (v17+) |

## 🚀 Pokretanje

### Preduslovi
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (preporučeno) ili bilo koji .NET IDE
- Windows OS (za WPF klijent)

### Konzolna aplikacija
```bash
cd ConsoleClient
dotnet run
```

### WPF aplikacija
```bash
cd WpfClient
dotnet run
```

### Ili kroz Visual Studio
1. Otvoriti `SajamKnjiga.sln` u Visual Studio-u
2. Izabrati željeni startup projekat (`ConsoleClient` ili `WpfClient`)
3. Kliknuti **Run** (F5)

> ⚠️ **Napomena za konzolnu verziju:** Obavezno izaći kroz opciju **5 (Sačuvaj i izađi)** kako bi se podaci sačuvali!

## 📊 Domenski model

```
Posetilac (R/V)  ──kupuje──▶  Knjiga  ◀──piše──  Autor
       │                         │                   │
       │                         │                   │
   KupljeneKnjige            Izdavač            SpisakKnjiga
   ListaZelja            (šef = Autor)
```

- **Posetilac** može kupiti više knjiga i imati listu želja
- **Knjiga** može imati više autora i pripada izdavaču
- **Izdavač** ima šefa (koji je Autor) i spisak autora/knjiga
- **Kupovina** povezuje Posetioca i Knjigu sa ocenom i komentarom

## 👥 Autori projekta

| Ime | Uloga |
|---|---|
| **Luka Avramović** | Student 1 |
| **Vukašin Petrović** | Student 2 |

## 📄 Licenca

Projekat je izrađen u akademske svrhe za predmet OISI.