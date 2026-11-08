using SajamKnjiga.Model;
using SajamKnjiga.Storage;

namespace SajamKnjiga.DAO;

class KnjigaDAO
{
    private readonly List<Knjiga> _knjige;
    private readonly Storage<Knjiga> _storage;

    public KnjigaDAO()
    {
        _storage = new Storage<Knjiga>("Knjiga.txt");
        _knjige = _storage.Load();
    }

    // Knjiga se identifikuje po ISBN-u, pa nema generisanja Id-ja.
    // Vraća null ako knjiga sa istim ISBN-om već postoji.
    public Knjiga? AddKnjiga(Knjiga knjiga)
    {
        if (GetKnjigaByIsbn(knjiga.Isbn) is not null) return null;

        _knjige.Add(knjiga);
        _storage.Save(_knjige);
        return knjiga;
    }

    public Knjiga? UpdateKnjiga(Knjiga knjiga)
    {
        Knjiga? staraKnjiga = GetKnjigaByIsbn(knjiga.Isbn);
        if (staraKnjiga is null) return null;

        staraKnjiga.Naziv = knjiga.Naziv;
        staraKnjiga.Zanr = knjiga.Zanr;
        staraKnjiga.GodinaIzdanja = knjiga.GodinaIzdanja;
        staraKnjiga.Cena = knjiga.Cena;
        staraKnjiga.BrojStrana = knjiga.BrojStrana;

        _storage.Save(_knjige);
        return staraKnjiga;
    }

    public Knjiga? RemoveKnjiga(string isbn)
    {
        Knjiga? knjiga = GetKnjigaByIsbn(isbn);
        if (knjiga is null) return null;

        _knjige.Remove(knjiga);
        _storage.Save(_knjige);
        return knjiga;
    }

    private Knjiga? GetKnjigaByIsbn(string isbn)
    {
        return _knjige.Find(k => k.Isbn == isbn);
    }

    public List<Knjiga> GetAllKnjige()
    {
        return _knjige;
    }

    public List<Knjiga> GetAllKnjige(int page, int pageSize, string sortCriteria, SortDirection sortDirection)
    {
        IEnumerable<Knjiga> knjige = _knjige;

        // sortiraj knjige ukoliko je sortCriteria naveden
        switch (sortCriteria)
        {
            case "Isbn":
                knjige = _knjige.OrderBy(k => k.Isbn);
                break;
            case "Naziv":
                knjige = _knjige.OrderBy(k => k.Naziv);
                break;
            case "Cena":
                knjige = _knjige.OrderBy(k => k.Cena);
                break;
            case "GodinaIzdanja":
                knjige = _knjige.OrderBy(k => k.GodinaIzdanja);
                break;
            case "Zanr":
                knjige = _knjige.OrderBy(k => k.Zanr);
                break;
        }

        // promijeni redoslijed ukoliko je opadajuće
        if (sortDirection == SortDirection.Descending)
            knjige = knjige.Reverse();

        // paginacija
        knjige = knjige.Skip((page - 1) * pageSize).Take(pageSize);

        return knjige.ToList();
    }
}
