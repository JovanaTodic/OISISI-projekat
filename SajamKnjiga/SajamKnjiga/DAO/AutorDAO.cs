using SajamKnjiga.Model;
using SajamKnjiga.Storage;

namespace SajamKnjiga.DAO;

class AutorDAO
{
    private readonly List<Autor> _autori;
    private readonly Storage<Autor> _storage;

    public AutorDAO()
    {
        _storage = new Storage<Autor>("Autor.txt");
        _autori = _storage.Load();
    }

    private int GenerateId()
    {
        if (_autori.Count == 0) return 1;
        return _autori.Max(a => a.Id) + 1;
    }

    public Autor AddAutor(Autor autor)
    {
        autor.Id = GenerateId();
        _autori.Add(autor);
        _storage.Save(_autori);
        return autor;
    }

    public Autor? UpdateAutor(Autor autor)
    {
        Autor? stariAutor = GetAutorById(autor.Id);
        if (stariAutor is null) return null;

        stariAutor.Ime = autor.Ime;
        stariAutor.Prezime = autor.Prezime;
        stariAutor.DatumRodjenja = autor.DatumRodjenja;
        stariAutor.Adresa = autor.Adresa;
        stariAutor.Telefon = autor.Telefon;
        stariAutor.E_mejl = autor.E_mejl;
        stariAutor.BrojLicneKarte = autor.BrojLicneKarte;
        stariAutor.GodineIskustva = autor.GodineIskustva;

        _storage.Save(_autori);
        return stariAutor;
    }

    public Autor? RemoveAutor(int id)
    {
        Autor? autor = GetAutorById(id);
        if (autor is null) return null;

        _autori.Remove(autor);
        _storage.Save(_autori);
        return autor;
    }

    private Autor? GetAutorById(int id)
    {
        return _autori.Find(a => a.Id == id);
    }

    public List<Autor> GetAllAutori()
    {
        return _autori;
    }

    public List<Autor> GetAllAutori(int page, int pageSize, string sortCriteria, SortDirection sortDirection)
    {
        IEnumerable<Autor> autori = _autori;

        // sortiraj autore ukoliko je sortCriteria naveden
        switch (sortCriteria)
        {
            case "Ime":
                autori = _autori.OrderBy(a => a.Ime);
                break;
            case "Prezime":
                autori = _autori.OrderBy(a => a.Prezime);
                break;
            case "E_mejl":
                autori = _autori.OrderBy(a => a.E_mejl);
                break;
        }

        // promijeni redoslijed ukoliko je opadajuće
        if (sortDirection == SortDirection.Descending)
            autori = autori.Reverse();

        // paginacija
        autori = autori.Skip((page - 1) * pageSize).Take(pageSize);

        return autori.ToList();
    }
}
