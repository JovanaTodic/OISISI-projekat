using SajamKnjiga.Serialization;

namespace SajamKnjiga.Model;

/*
 * Pomocna klasa koja predstavlja jedan red u fajlu knjige_autori.csv.
 * Jedan red = jedan par (knjiga, autor).
 */
public class KnjigaAutor : ISerializable
{
    public int KnjigaId { get; set; }
    public int AutorId { get; set; }

    public KnjigaAutor()
    {
    }

    public KnjigaAutor(int knjigaId, int autorId)
    {
        KnjigaId = knjigaId;
        AutorId = autorId;
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            KnjigaId.ToString(),
            AutorId.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        KnjigaId = int.Parse(values[0]);
        AutorId = int.Parse(values[1]);
    }
}

