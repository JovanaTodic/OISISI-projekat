using SajamKnjiga.Model;
using SajamKnjiga.Storage;

namespace SajamKnjiga.DAO;

class PosetilacDAO
{
    private readonly List<Posetilac> _posetioci;
    private readonly Storage<Posetilac> _storage;
    private readonly KupovinaDAO _kupovinaDao;
    private readonly AdresaDAO _adresaDao;

    public PosetilacDAO(KupovinaDAO kupovinaDao, AdresaDAO adresaDao)
    {
        _kupovinaDao = kupovinaDao;
        _adresaDao = adresaDao;
        _storage = new Storage<Posetilac>("Posetilac.txt");
        _posetioci = _storage.Load(); //od ovog trenutka su objekti u memoriji

        foreach (Posetilac p in _posetioci)
            p.Adresa = _adresaDao.GetAdresaById(p.AdresaId) ?? new Adresa(0, "-", "-", "-", "-");

        OsveziKupovine();
    }

    private int GenerateId()
    {
        if (_posetioci.Count == 0) return 1;
        return _posetioci.Max(p => p.Id) + 1;
    }

    // Broj clanske karte mora biti jedinstven
    private bool BrojKarteZauzet(string brojKarte, int ignorisiId)
    {
        return _posetioci.Any(p => p.Id != ignorisiId &&
                                   string.Equals(p.BrojClanskeKarte, brojKarte, StringComparison.OrdinalIgnoreCase));
    }

    // Vraca null ako je broj clanske karte vec zauzet.
    public Posetilac? AddPosetilac(Posetilac posetilac)
    {
        if (BrojKarteZauzet(posetilac.BrojClanskeKarte, 0)) return null;

        posetilac.Id = GenerateId();
        posetilac.Adresa = _adresaDao.AddAdresa(posetilac.Adresa);   // adresa se upisuje u Adresa.txt
        _posetioci.Add(posetilac);
        _storage.Save(_posetioci); //serijalizujemo
        return posetilac;
    }

    public Posetilac? UpdatePosetilac(Posetilac posetilac)
    {
        Posetilac? stariPosetilac = GetPosetilacById(posetilac.Id);
        if (stariPosetilac is null) return null;
        if (BrojKarteZauzet(posetilac.BrojClanskeKarte, posetilac.Id)) return null;

        stariPosetilac.Ime = posetilac.Ime;
        stariPosetilac.Prezime = posetilac.Prezime;
        stariPosetilac.DatumRodjenja = posetilac.DatumRodjenja;
        posetilac.Adresa.Id = stariPosetilac.Adresa.Id;
        _adresaDao.UpdateAdresa(posetilac.Adresa);   // mijenja istu adresu koju posetilac vec ima
        stariPosetilac.Telefon = posetilac.Telefon;
        stariPosetilac.Email = posetilac.Email;
        stariPosetilac.BrojClanskeKarte = posetilac.BrojClanskeKarte;
        stariPosetilac.GodinaClanstva = posetilac.GodinaClanstva;
        stariPosetilac.Status = posetilac.Status;

        _storage.Save(_posetioci);
        return stariPosetilac;
    }

    // Brisanjem posetioca brisu se i sve njegove kupovine
    public Posetilac? RemovePosetilac(int id)
    {
        Posetilac? posetilac = GetPosetilacById(id);
        if (posetilac is null) return null;

        _kupovinaDao.RemoveKupovineByPosetilac(id);
        _adresaDao.RemoveAdresa(posetilac.Adresa.Id);
        _posetioci.Remove(posetilac);
        _storage.Save(_posetioci);
        return posetilac;
    }

    public Posetilac? GetPosetilacById(int id)
    {
        return _posetioci.Find(p => p.Id == id);
    }

    public List<Posetilac> GetAllPosetioci()
    {
        return _posetioci;
    }

    public List<Posetilac> GetAllPosetioci(int page, int pageSize, string sortCriteria, SortDirection sortDirection)
    {
        IEnumerable<Posetilac> posetioci = _posetioci;

        
        switch (sortCriteria)
        {
            case "BrojClanskeKarte":
                posetioci = _posetioci.OrderBy(p => p.BrojClanskeKarte);
                break;
            case "Ime":
                posetioci = _posetioci.OrderBy(p => p.Ime);
                break;
            case "Prezime":
                posetioci = _posetioci.OrderBy(p => p.Prezime);
                break;
            case "GodinaClanstva":
                posetioci = _posetioci.OrderBy(p => p.GodinaClanstva);
                break;
            case "Status":
                posetioci = _posetioci.OrderBy(p => p.Status);
                break;
            case "ProsecnaOcena":
                posetioci = _posetioci.OrderBy(p => p.ProsecnaOcena);
                break;
        }

        // promijeni redoslijed ukoliko je opadajuće
        if (sortDirection == SortDirection.Descending)
            posetioci = posetioci.Reverse();

        // paginacija
        posetioci = posetioci.Skip((page - 1) * pageSize).Take(pageSize);

        return posetioci.ToList();
    }

    // Kupovine se u fajlu cuvaju odvojeno (samo ID posetioca i ID knjige).
    // Ovde se kupovine pridruzuju posetiocima i racuna se prosecna ocena.
    public void OsveziKupovine()
    {
        foreach (Posetilac p in _posetioci)
        {
            p.Kupovine.Clear();
            foreach (Kupovina kupovina in _kupovinaDao.GetAllKupovine().Where(x => x.PosetilacId == p.Id))
            {
                kupovina.Posetilac = p;
                p.Kupovine.Add(kupovina);
            }
            p.AzurirajProsecnuOcenu();
        }
    }
}
