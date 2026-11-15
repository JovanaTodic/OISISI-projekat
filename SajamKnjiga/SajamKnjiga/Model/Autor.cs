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
    public string E_mail { get; set; }
    public string BrojLicneKarte { get; set; }
    public int GodineIskustva { get; set; }
    public List<Knjiga> Knjige { get; } = new();
    public int AdresaId { get; set; }   // pomocno polje, adresa se povezuje u AutorDAO

    public Autor()
    {
    }

    public Autor(int id, string ime, string prezime, DateTime datumRodjenja, Adresa adresa, string telefon, string e_mail, string brojLicneKarte, int godineIskustva)
    {
        Id = id;
        Ime = ime;
        Prezime = prezime;
        DatumRodjenja = datumRodjenja;
        Adresa = adresa;
        Telefon = telefon;
        E_mail = e_mail;
        BrojLicneKarte = brojLicneKarte;
        GodineIskustva = godineIskustva;

    }

    public override string ToString()
    {
        return $"ID: {Id,3} | {Ime,-12} {Prezime,-15} | Rodjen: {DatumRodjenja} | " +
               $"LK: {BrojLicneKarte,-9} | Iskustvo: {GodineIskustva,2} | E-mail: {E_mail}";
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
        E_mail,
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
        E_mail = values[5];
        BrojLicneKarte = values[6];
        GodineIskustva = int.Parse(values[7]);
        AdresaId = int.Parse(values[8]);
    }







}
