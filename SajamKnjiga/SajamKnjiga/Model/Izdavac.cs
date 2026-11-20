using SajamKnjiga.Serialization;

namespace SajamKnjiga.Model;

public class Izdavac : ISerializable
{
    public int Id { get; set; }
    public int Sifra { get; set; }
    public string Naziv { get; set; }
    public Autor? Sef { get; set; }              // autor sa najmanje 5 godina iskustva
    public IEnumerable<Autor> Autori => Knjige.SelectMany(k => k.Autori).Distinct();
    public List<Knjiga> Knjige { get; } = new();

    
    public int SefId { get; set; }

    public const int MinGodinaIskustvaSefa = 5;

    public Izdavac()
    {
    }

    public Izdavac(int id, int sifra, string naziv)
    {
        Id = id;
        Sifra = sifra;
        Naziv = naziv;
    }

    public static bool MozeBitiSef(Autor autor) => autor.GodineIskustva >= MinGodinaIskustvaSefa;

    public override string ToString()
    {
        string sef = Sef == null ? "nema" : $"{Sef.Ime} {Sef.Prezime}";
        int brojAutora = Autori.Count();
        return $"Id: {Id,3} |Sifra: {Sifra,3} | {Naziv,-20} | Sef: {sef,-25} | Autora: {brojAutora} | Knjiga: {Knjige.Count}";
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            Sifra.ToString(),
            Naziv,
            SefId.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Sifra = int.Parse(values[1]);
        Naziv = values[2];
        SefId = int.Parse(values[3]);
    }
}

