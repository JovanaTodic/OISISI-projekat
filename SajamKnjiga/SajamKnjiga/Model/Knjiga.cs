using SajamKnjiga.Serialization;
using System.Globalization;


namespace SajamKnjiga.Model;

public class Knjiga : ISerializable
{
    public string Isbn { get; set; }
    public string Naziv { get; set; }
    public string Zanr { get; set; }
    public int GodinaIzdanja { get; set; }
    public decimal Cena { get; set; }
    public int BrojStrana { get; set; }
    public List<Autor> autori { get; set; } = new();
    public Izdavac Izdavac { get; set; }
    public List<Posetilac> PosetiociKupili { get; set; } = new();
    public List<Posetilac> PosetiociListaZelja { get; set; } = new();

    public Knjiga()
    {
    }

    public Knjiga(string isbn, string naziv, string zanr, int godinaIzdanja, decimal cena, int brojStrana)
    {
        Isbn = isbn;
        Naziv = naziv;
        Zanr = zanr;
        GodinaIzdanja = godinaIzdanja;
        Cena = cena;
        BrojStrana = brojStrana;
    }

    public override string ToString()
    {
        return $"{Isbn} | {Naziv} | {Zanr} | {GodinaIzdanja} | {Cena:0.00} | {BrojStrana}";
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Isbn,
            Naziv,
            Zanr,
            GodinaIzdanja.ToString(),
            Cena.ToString(CultureInfo.InvariantCulture),
            BrojStrana.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Isbn = values[0];
        Naziv = values[1];
        Zanr = values[2];
        GodinaIzdanja = int.Parse(values[3]);
        Cena = decimal.Parse(values[4], CultureInfo.InvariantCulture);
        BrojStrana = int.Parse(values[5]);
    }
}

