using SajamKnjiga.Model;
using SajamKnjiga.Storage;

namespace SajamKnjiga.DAO;

class AutorDAO
{
    private readonly List<Autor> _autor;
    private readonly Storage<Autor> _storage;
    private readonly AdresaDAO _adresaDao;

    public AutorDAO(AdresaDAO adresaDao)
    {
        _adresaDao = adresaDao;
        _storage = new Storage<Autor>("Autor.txt");
        _autor = _storage.Load();

        // u fajlu se cuva samo ID adrese, pa se ovde pronalazi prava adresa
        foreach (Autor autor in _autor)
            autor.Adresa = _adresaDao.GetAdresaById(autor.AdresaId) ?? new Adresa(0, "-", "-", "-", "-");
    }

    private int GenerateId()
    {
        if (_autor.Count == 0) return 1;
        return _autor.Max(a => a.Id) + 1;
    }

    public Autor AddAutor(Autor autor)
    {
        autor.Id = GenerateId();
        autor.Adresa = _adresaDao.AddAdresa(autor.Adresa);   // adresa se upisuje u Adresa.txt
        _autor.Add(autor);
        _storage.Save(_autor);
        return autor;
    }

    public Autor? UpdateAutor(Autor autor)
    {
        Autor? stariAutor = GetAutorById(autor.Id);
        if (stariAutor is null) return null;

        stariAutor.Ime = autor.Ime;
        stariAutor.Prezime = autor.Prezime;
        stariAutor.DatumRodjenja = autor.DatumRodjenja;
        autor.Adresa.Id = stariAutor.Adresa.Id;
        _adresaDao.UpdateAdresa(autor.Adresa);   // mijenja istu adresu koju autor vec ima
        stariAutor.Telefon = autor.Telefon;
        stariAutor.Email = autor.Email;
        stariAutor.BrojLicneKarte = autor.BrojLicneKarte;
        stariAutor.GodineIskustva = autor.GodineIskustva;

        _storage.Save(_autor);
        return stariAutor;
    }

    public Autor? RemoveAutor(int id)
    {
        Autor? autor = GetAutorById(id);
        if (autor is null) return null;

        _adresaDao.RemoveAdresa(autor.Adresa.Id);
        _autor.Remove(autor);
        _storage.Save(_autor);
        return autor;
    }

    public Autor? GetAutorById(int id)
    {
        return _autor.Find(a => a.Id == id);
    }

    public List<Autor> GetAllAutori()
    {
        return _autor;
    }

    public List<Autor> GetAllAutori(int page, int pageSize, string sortCriteria, SortDirection sortDirection)
    {
        IEnumerable<Autor> autori = _autor;

        // sortiraj autore ukoliko je sortCriteria naveden
        switch (sortCriteria)
        {
            case "Ime":
                autori = _autor.OrderBy(a => a.Ime);
                break;
            case "Prezime":
                autori = _autor.OrderBy(a => a.Prezime);
                break;
            case "Email":
                autori = _autor.OrderBy(a => a.Email);
                break;
        }

        // promijeni redoslijed ukoliko je opadajuće
        if (sortDirection == SortDirection.Descending)
            autori = autori.Reverse();

        // paginacija
        autori = autori.Skip((page - 1) * pageSize).Take(pageSize);

        return autori.ToList();
    }

    public bool JeJedinAutorNekeKnjige(Autor autor)
    {
        return autor.Knjige.Any(k => k.Autori.Count == 1);
    }
}
