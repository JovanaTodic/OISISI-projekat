using SajamKnjiga.Serialization;

namespace SajamKnjiga.Model;

public class Kupovina : ISerializable
{
    public int Id { get; set; }
    public Posetilac Posetilac { get; set; }
    public Knjiga Knjiga { get; set; }
    public DateTime DatumKupovine { get; set; }
    public int Ocena { get; set; }          // 1-5
    public string Komentar { get; set; }

    // Pomocna polja: u fajlu se cuvaju samo ID posetioca i ISBN knjige,
    public int PosetilacId { get; set; }
    public string KnjigaIsbn { get; set; }

    public Kupovina()
    {
    }

    public Kupovina(int id, Posetilac posetilac, Knjiga knjiga, DateTime datumKupovine, int ocena, string komentar)
    {
        Id = id;
        Posetilac = posetilac;
        Knjiga = knjiga;
        PosetilacId = posetilac.Id;
        KnjigaIsbn = knjiga.Isbn;
        DatumKupovine = datumKupovine;
        Ocena = ocena;
        Komentar = komentar;
    }

    public override string ToString()
    {
        return $"ID: {Id,3} | Posetilac: {PosetilacId,3} | ISBN: {KnjigaIsbn,-14} | " +
               $"Datum: {DatumKupovine:dd.MM.yyyy.} | Ocena: {Ocena} | Komentar: {Komentar}";
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            PosetilacId.ToString(),
            KnjigaIsbn,
            DatumKupovine.ToString("yyyy-MM-dd"),
            Ocena.ToString(),
            (Komentar ?? "").Replace("|", "/")
            //ako je promenjiva prazan string upotrebi "" i zamijenio sam uspravnu sa kosom da se ne
            //desi da unese komentar sa | i on to ne prepozna kao komentar nego isjece liniju po tome jer pomisli da je granica
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        PosetilacId = int.Parse(values[1]);
        KnjigaIsbn = values[2];
        DatumKupovine = DateTime.ParseExact(values[3], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        Ocena = int.Parse(values[4]);
        Komentar = values[5];
    }
}
