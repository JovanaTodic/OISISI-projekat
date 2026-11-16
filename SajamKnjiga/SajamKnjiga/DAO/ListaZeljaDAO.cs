using SajamKnjiga.Model;
using SajamKnjiga.Storage;
namespace SajamKnjiga.DAO;

class ListaZeljaDAO
{
    private readonly Storage<ListaZeljaStavka> _storage = new("lista_zelja.txt");
    private readonly PosetilacDAO _posetilacDao;

    public ListaZeljaDAO(PosetilacDAO posetilacDao, KnjigaDAO knjigaDao)
    {
        _posetilacDao = posetilacDao;
        foreach (ListaZeljaStavka s in _storage.Load())
        {
            Posetilac? p = posetilacDao.GetPosetilacById(s.PosetilacId);
            Knjiga? k = knjigaDao.GetKnjigaByIsbn(s.Isbn);
            if (p is null || k is null) continue;      // preskoči nepostojeće
            if (!p.ListaZelja.Contains(k)) p.ListaZelja.Add(k);
            if (!k.PosetiociListaZelja.Contains(p)) k.PosetiociListaZelja.Add(p);
        }
    }

    public bool Dodaj(Posetilac p, Knjiga k)
    {
        if (p.ListaZelja.Contains(k)) return false;
        p.ListaZelja.Add(k);
        k.PosetiociListaZelja.Add(p);
        Sacuvaj();
        return true;
    }

    public bool Ukloni(Posetilac p, Knjiga k)
    {
        if (!p.ListaZelja.Remove(k)) return false;
        k.PosetiociListaZelja.Remove(p);
        Sacuvaj();
        return true;
    }

    // pozvati nakon brisanja posetioca
    public void UkloniSvePosetioca(Posetilac p)
    {
        foreach (Knjiga k in p.ListaZelja) k.PosetiociListaZelja.Remove(p);
        p.ListaZelja.Clear();
        Sacuvaj();
    }

    // pozvati nakon brisanja knjige
    public void UkloniSveKnjige(Knjiga k)
    {
        foreach (Posetilac p in k.PosetiociListaZelja) p.ListaZelja.Remove(k);
        k.PosetiociListaZelja.Clear();
        Sacuvaj();
    }

    private void Sacuvaj()
    {
        var stavke = _posetilacDao.GetAllPosetioci()
            .SelectMany(p => p.ListaZelja.Select(k => new ListaZeljaStavka(p.Id, k.Isbn)))
            .ToList();
        _storage.Save(stavke);
    }
}
