using SajamKnjiga.Serialization;
using System.Globalization;

namespace SajamKnjiga.Model;

public class Autor : ISerializable
{
    public int Id { get; set; }
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public DateTime DatumRodjenja { get; set; }
    public Adresa Adresa { get; set; }
    public string Telefon { get; set; }
    public string Email { get; set; }
    public string BrojLicneKarte { get; set; }
    public int GodineIskustva { get; set; }
    public List<Knjiga> Knjige { get; set; } = new();
    public int AdresaId { get; set; }   // pomocno polje, adresa se povezuje u AutorDAO

    public Autor()
    {
    }

    public Autor(int id, string ime, string prezime, DateTime datumRodjenja, Adresa adresa, string telefon, string email, string brojLicneKarte, int godineIskustva)
    {
        Id = id;
        Ime = ime;
        Prezime = prezime;
        DatumRodjenja = datumRodjenja;
        Adresa = adresa;
        Telefon = telefon;
        Email = email;
        BrojLicneKarte = brojLicneKarte;
        GodineIskustva = godineIskustva;

    }

    public override string ToString()
    {
        string knjige = Knjige.Count > 0
            ? string.Join(", ", Knjige.Select(k => k.Naziv))
            : "nema knjiga";

        return $"ID: {Id,3} | {Ime,-12} {Prezime,-15} | Rodjen: {DatumRodjenja:yyyy-MM-dd} | " +
               $"LK: {BrojLicneKarte,-9} | Iskustvo: {GodineIskustva,2} | Email: {Email} | Knjige: {knjige}";
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
        Id.ToString(),
        Ime,
        Prezime,
        DatumRodjenja.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        Telefon,
        Email,
        BrojLicneKarte,
        GodineIskustva.ToString(),
        Adresa.Id.ToString()
    };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Ime = values[1];
        Prezime = values[2];
        DatumRodjenja = DateTime.ParseExact(values[3], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        Telefon = values[4];
        Email = values[5];
        BrojLicneKarte = values[6];
        GodineIskustva = int.Parse(values[7]);
        AdresaId = int.Parse(values[8]);
    }









}
