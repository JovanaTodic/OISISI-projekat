using SajamKnjiga.Serialization;

namespace SajamKnjiga.Model;

public enum StatusPosetioca
{
    Redovan,   // "R"
    Vip        // "V"
}

public class Posetilac : ISerializable
{
    public int Id { get; set; }
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public DateTime DatumRodjenja { get; set; }
    public Adresa Adresa { get; set; }
    public string Telefon { get; set; }
    public string E_mejl { get; set; }
    public string BrojClanskeKarte { get; set; }   // npr. CK-1-2023
    public int GodinaClanstva { get; set; }
    public StatusPosetioca Status { get; set; }
    public double ProsecnaOcena { get; set; }
    public List<Kupovina> Kupovine { get; } = new();
    public List<Knjiga> ListaZelja { get; } = new();

    public Posetilac()
    {
    }

    public Posetilac(int id, string ime, string prezime, DateTime datumRodjenja, Adresa adresa,
                     string telefon, string e_mejl, string brojClanskeKarte, int godinaClanstva,
                     StatusPosetioca status)
    {
        Id = id;
        Ime = ime;
        Prezime = prezime;
        DatumRodjenja = datumRodjenja;
        Adresa = adresa;
        Telefon = telefon;
        E_mejl = e_mejl;
        BrojClanskeKarte = brojClanskeKarte;
        GodinaClanstva = godinaClanstva;
        Status = status;
    }

    // Prosecna ocena se racuna iz kupovina (vidi KupovinaDAO / PosetilacDAO)
    public void AzurirajProsecnuOcenu()
    {
        ProsecnaOcena = Kupovine.Count == 0 ? 0 : Kupovine.Average(k => k.Ocena);
    }

    public override string ToString()
    {
        string status = Status == StatusPosetioca.Vip ? "V" : "R";
        return $"ID: {Id,3} | {BrojClanskeKarte,-12} | {Ime,-12} {Prezime,-15} | " +
               $"Rodjen: {DatumRodjenja:dd.MM.yyyy.} | Tel: {Telefon,-12} | E-mail: {E_mejl,-22} | " +
               $"Clanstvo: {GodinaClanstva} | Status: {status} | Ocena: {ProsecnaOcena:0.00}";
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            Ime,
            Prezime,
            DatumRodjenja.ToString("yyyy-MM-dd"),
            Telefon,
            E_mejl,
            BrojClanskeKarte,
            GodinaClanstva.ToString(),
            Status == StatusPosetioca.Vip ? "V" : "R",
            Adresa.Id.ToString(),
            Adresa.Ulica,
            Adresa.Broj,
            Adresa.Grad,
            Adresa.Drzava
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Ime = values[1];
        Prezime = values[2];
        DatumRodjenja = DateTime.ParseExact(values[3], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        Telefon = values[4];
        E_mejl = values[5];
        BrojClanskeKarte = values[6];
        GodinaClanstva = int.Parse(values[7]);
        Status = values[8] == "V" ? StatusPosetioca.Vip : StatusPosetioca.Redovan;
        Adresa = new Adresa(int.Parse(values[9]), values[10], values[11], values[12], values[13]);
    }
}

