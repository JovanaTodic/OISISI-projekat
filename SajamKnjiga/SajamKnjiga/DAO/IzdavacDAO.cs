using SajamKnjiga.Model;
using SajamKnjiga.Storage;

namespace SajamKnjiga.DAO;

class IzdavacDAO
{
    private readonly List<Izdavac> _izdavaci;
    private readonly Storage<Izdavac> _storage;
    private readonly AutorDAO _autorDao;

    public IzdavacDAO(AutorDAO autorDao)
    {
        _autorDao = autorDao;
        _storage = new Storage<Izdavac>("Izdavac.txt");
        _izdavaci = _storage.Load();

        // u fajlu se cuva samo ID sefa, pa se ovde pronalazi pravi autor
        foreach (Izdavac izdavac in _izdavaci)
            izdavac.Sef = _autorDao.GetAllAutori().Find(a => a.Id == izdavac.SefId);
    }

    private int GenerateSifra()
    {
        if (_izdavaci.Count == 0) return 1;
        return _izdavaci.Max(i => i.Sifra) + 1;
    }

    public Izdavac AddIzdavac(Izdavac izdavac)
    {
        izdavac.Sifra = GenerateSifra();
        _izdavaci.Add(izdavac);
        _storage.Save(_izdavaci);
        return izdavac;
    }

    // Menja se samo naziv, sef se postavlja preko PostaviSefa
    public Izdavac? UpdateIzdavac(Izdavac izdavac)
    {
        Izdavac? stariIzdavac = GetIzdavacBySifra(izdavac.Sifra);
        if (stariIzdavac is null) return null;

        stariIzdavac.Naziv = izdavac.Naziv;

        _storage.Save(_izdavaci);
        return stariIzdavac;
    }

    public Izdavac? RemoveIzdavac(int sifra)
    {
        Izdavac? izdavac = GetIzdavacBySifra(sifra);
        if (izdavac is null) return null;

        _izdavaci.Remove(izdavac);
        _storage.Save(_izdavaci);
        return izdavac;
    }

    // Sef mora imati najmanje 5 godina iskustva. Vraca false ako uslov nije ispunjen.
    public bool PostaviSefa(Izdavac izdavac, Autor autor)
    {
        if (!Izdavac.MozeBitiSef(autor)) return false;

        izdavac.Sef = autor;
        izdavac.SefId = autor.Id;
        _storage.Save(_izdavaci);
        return true;
    }

    public Izdavac? GetIzdavacBySifra(int sifra)
    {
        return _izdavaci.Find(i => i.Sifra == sifra);
    }

    // Poziva se pri brisanju autora (referencijalna zavisnost):
    // izdavaci kojima je taj autor bio sef ostaju bez sefa.
    public void UkloniSefa(int autorId)
    {
        bool promijenjeno = false;
        foreach (Izdavac izdavac in _izdavaci.Where(i => i.SefId == autorId))
        {
            izdavac.Sef = null;
            izdavac.SefId = 0;
            promijenjeno = true;
        }

        if (promijenjeno) _storage.Save(_izdavaci);
    }

    public List<Izdavac> GetAllIzdavaci()
    {
        return _izdavaci;
    }
}
