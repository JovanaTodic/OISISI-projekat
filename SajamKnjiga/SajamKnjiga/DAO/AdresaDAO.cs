using SajamKnjiga.Model;
using SajamKnjiga.Storage;

namespace SajamKnjiga.DAO;

class AdresaDAO
{
   private readonly List<Adresa> _adresa;
   private readonly Storage<Adresa> _storage;


    public AdresaDAO()
    {
        _storage = new Storage<Adresa>("Adresa.txt");
        _adresa = _storage.Load();
    }

    private int GenerateId()
    {
        if (_adresa.Count == 0) return 0;
        return _adresa[^1].Id + 1;
    }

    public Adresa AddAdresa(Adresa adresa)
    {
        adresa.Id = GenerateId();
        _adresa.Add(adresa);
        _storage.Save(_adresa);
        return adresa;
    }

    public Adresa? UpdateAdresa(Adresa adresa)
    {
        Adresa? staraAdresa = GetAdresaById(adresa.Id);
        if (staraAdresa is null) return null;

        staraAdresa.Ulica = adresa.Ulica;
        staraAdresa.Broj = adresa.Broj;
        staraAdresa.Grad = adresa.Grad;
        staraAdresa.Drzava = adresa.Drzava;


        _storage.Save(_adresa);
        return staraAdresa;
    }

    public Adresa? RemoveAdresa(int id)
    {
        Adresa? adresa = GetAdresaById(id);
        if (adresa == null) return null;

        _adresa.Remove(adresa);
        _storage.Save(_adresa);
        return adresa;
    }

    private Adresa? GetAdresaById(int id)
    {
        return _adresa.Find(v => v.Id == id);
    }


    public List<Adresa> GetAllAdrese()
    {
        return _adresa;
    }

    public List<Adresa> GetAllAdrese(int page, int pageSize, string sortCriteria, SortDirection sortDirection)
    {
        IEnumerable<Adresa> adresa = _adresa;

        // sortiraj adrese ukoliko je sortCriteria naveden
        switch (sortCriteria)
        {
            case "Id":
                adresa = _adresa.OrderBy(a => a.Id);
                break;
            case "Ulica":
                adresa = _adresa.OrderBy(a => a.Ulica);
                break;
            case "Broj":
                adresa = _adresa.OrderBy(a => a.Broj);
                break;
            case "Grad":
                adresa = _adresa.OrderBy(a => a.Grad);
                break;
            case "Drzava":
                adresa = _adresa.OrderBy(a => a.Drzava);
                break;
        }

        // promijeni redoslijed ukoliko je opadajuće
        if (sortDirection == SortDirection.Descending)
            adresa = adresa.Reverse();

        // paginacija
        adresa = adresa.Skip((page - 1) * pageSize).Take(pageSize);

        return adresa.ToList();
    }

}
