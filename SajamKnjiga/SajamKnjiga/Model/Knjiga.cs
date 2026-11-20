using SajamKnjiga.Serialization;
using System.Globalization;


namespace SajamKnjiga.Model;

public class Knjiga : ISerializable
{
    public int Id { get; set; }
    public string Isbn { get; set; }
    public string Naziv { get; set; }
    public string Zanr { get; set; }
    public int GodinaIzdanja { get; set; }
    public decimal Cena { get; set; }
    public int BrojStrana { get; set; }
    public List<Autor> Autori { get; set; } = new();
    public Izdavac? Izdavac { get; set; }

    public int IzdavacId { get; set; }
    public List<Posetilac> PosetiociKupili { get; set; } = new();
    public List<Posetilac> PosetiociListaZelja { get; set; } = new();

    public Knjiga()
    {
    }

    public Knjiga(int id, string isbn, string naziv, string zanr, int godinaIzdanja, decimal cena, int brojStrana)
    {
        Id = id;
        Isbn = isbn;
        Naziv = naziv;
        Zanr = zanr;
        GodinaIzdanja = godinaIzdanja;
        Cena = cena;
        BrojStrana = brojStrana;
    }

    public override string ToString()
    {
        string autori = Autori.Count > 0
            ? string.Join(", ", Autori.Select(a => $"{a.Ime} {a.Prezime}"))
            : "nema autora";
        string izdavac = Izdavac?.Naziv ?? "nema izdavača";

        return $"{Id,3} | {Isbn} | {Naziv} | {Zanr} | {GodinaIzdanja} | {Cena:0.00} | {BrojStrana} | Izdavač: {izdavac} | Autori: {autori}";
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            Isbn,
            Naziv,
            Zanr,
            GodinaIzdanja.ToString(),
            Cena.ToString(CultureInfo.InvariantCulture),
            BrojStrana.ToString(),
            IzdavacId.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Isbn = values[1];
        Naziv = values[2];
        Zanr = values[3];
        GodinaIzdanja = int.Parse(values[4]);
        Cena = decimal.Parse(values[5], CultureInfo.InvariantCulture);
        BrojStrana = int.Parse(values[6]);
        IzdavacId = int.Parse(values[7]);
    }
}

