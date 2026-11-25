using SajamKnjiga.Model;
using SajamKnjiga.Storage;

namespace SajamKnjiga.DAO;

public class KnjigaDAO
{
    private readonly List<Knjiga> _knjige;
    private readonly Storage<Knjiga> _storage;
    private readonly KupovinaDAO _kupovinaDao;

    private readonly IzdavacDAO _izdavacDao;

    public KnjigaDAO(KupovinaDAO kupovinaDao, IzdavacDAO izdavacDao)
    {
        _kupovinaDao = kupovinaDao;
        _izdavacDao = izdavacDao;
        _storage = new Storage<Knjiga>("Knjiga.txt");
        _knjige = _storage.Load();

        foreach (Knjiga k in _knjige)
        {
            k.Izdavac = _izdavacDao.GetIzdavacById(k.IzdavacId);
            k.Izdavac?.Knjige.Add(k);
        }

    }

    private int GenerateId()
    {
        if (_knjige.Count == 0) return 1;
        return _knjige.Max(k => k.Id) + 1;
    }

    public bool IsbnPostoji(string isbn, int ignoreId = 0)
    {
        return _knjige.Any(k => k.Isbn == isbn && k.Id != ignoreId);
    }

    public Knjiga? AddKnjiga(Knjiga knjiga)
    {
        if (IsbnPostoji(knjiga.Isbn)) return null;
        Izdavac? izdavac = _izdavacDao.GetIzdavacById(knjiga.IzdavacId);
        if (izdavac is null) return null;

        knjiga.Id = GenerateId();
        knjiga.Izdavac = izdavac;
        izdavac.Knjige.Add(knjiga);         
        _knjige.Add(knjiga);
        _storage.Save(_knjige);
        return knjiga;
    }

    public Knjiga? UpdateKnjiga(Knjiga knjiga)
    {
        Knjiga? staraKnjiga = GetKnjigaById(knjiga.Id);
        if (staraKnjiga is null) return null;
        if (IsbnPostoji(knjiga.Isbn, knjiga.Id)) return null;

        // prvo proveri izdavača, pa tek onda menjaj polja
        Izdavac? novi = _izdavacDao.GetIzdavacById(knjiga.IzdavacId);
        if (novi is null) return null;

        staraKnjiga.Isbn = knjiga.Isbn;
        staraKnjiga.Naziv = knjiga.Naziv;
        staraKnjiga.Zanr = knjiga.Zanr;
        staraKnjiga.GodinaIzdanja = knjiga.GodinaIzdanja;
        staraKnjiga.Cena = knjiga.Cena;
        staraKnjiga.BrojStrana = knjiga.BrojStrana;

        staraKnjiga.Izdavac?.Knjige.Remove(staraKnjiga);
        staraKnjiga.Izdavac = novi;
        staraKnjiga.IzdavacId = novi.Id;
        novi.Knjige.Add(staraKnjiga);

        _storage.Save(_knjige);
        return staraKnjiga;
    }

    public Knjiga? RemoveKnjiga(int id)
    {
        Knjiga? knjiga = GetKnjigaById(id);
        if (knjiga is null) return null;

        _kupovinaDao.RemoveKupovineByKnjiga(id);
        knjiga.Izdavac?.Knjige.Remove(knjiga);
        foreach (Autor a in knjiga.Autori) a.Knjige.Remove(knjiga);
        _knjige.Remove(knjiga);
        _storage.Save(_knjige);
        return knjiga;
    }

    public Knjiga? GetKnjigaById(int id)
    {
        return _knjige.Find(k => k.Id == id);
    }

    public Knjiga? GetKnjigaByIsbn(string isbn)
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
            case "Id":
                knjige = _knjige.OrderBy(k => k.Id);
                break;
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

    public void OsveziKupovine()
    {
        foreach (Knjiga k in _knjige) k.PosetiociKupili.Clear();
        foreach (Kupovina kup in _kupovinaDao.GetAllKupovine())
        {
            Knjiga? k = GetKnjigaById(kup.KnjigaId);
            if (k is null) continue;
            kup.Knjiga = k;
            if (kup.Posetilac is not null) k.PosetiociKupili.Add(kup.Posetilac);
        }
    }
}
