using SajamKnjiga.Serialization;

namespace SajamKnjiga.Model;

/*
 * Pomocna klasa koja predstavlja jedan red u fajlu knjige_autori.csv.
 * Jedan red = jedan par (knjiga, autor).
 */
public class KnjigaAutor : ISerializable
{
    public string Isbn { get; set; }
    public int AutorId { get; set; }

    public KnjigaAutor()
    {
    }

    public KnjigaAutor(string isbn, int autorId)
    {
        Isbn = isbn;
        AutorId = autorId;
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Isbn,
            AutorId.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Isbn = values[0];
        AutorId = int.Parse(values[1]);
    }
}

