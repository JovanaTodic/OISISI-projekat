using SajamKnjiga.Model;
using SajamKnjiga.Storage;

namespace SajamKnjiga.DAO;

class KupovinaDAO
{
    private readonly List<Kupovina> _kupovine;
    private readonly Storage<Kupovina> _storage;

    public KupovinaDAO()
    {
        _storage = new Storage<Kupovina>("Kupovina.txt");
        _kupovine = _storage.Load();
    }

    private int GenerateId()
    {
        if (_kupovine.Count == 0) return 1;
        return _kupovine.Max(k => k.Id) + 1;
    }

    // Vraca null ako je ocena van opsega 1-5 ili je posetilac vec kupio tu knjigu.
    public Kupovina? AddKupovina(Kupovina kupovina)
    {
        if (kupovina.Ocena < 1 || kupovina.Ocena > 5) return null;
        if (_kupovine.Any(k => k.PosetilacId == kupovina.PosetilacId && k.KnjigaIsbn == kupovina.KnjigaIsbn))
            return null;

        kupovina.Id = GenerateId();
        _kupovine.Add(kupovina);
        _storage.Save(_kupovine);
        return kupovina;
    }

    public Kupovina? UpdateKupovina(Kupovina kupovina)
    {
        Kupovina? staraKupovina = GetKupovinaById(kupovina.Id);
        if (staraKupovina is null) return null;
        if (kupovina.Ocena < 1 || kupovina.Ocena > 5) return null;

        staraKupovina.DatumKupovine = kupovina.DatumKupovine;
        staraKupovina.Ocena = kupovina.Ocena;
        staraKupovina.Komentar = kupovina.Komentar;

        _storage.Save(_kupovine);
        return staraKupovina;
    }

    public Kupovina? RemoveKupovina(int id)
    {
        Kupovina? kupovina = GetKupovinaById(id);
        if (kupovina is null) return null;

        _kupovine.Remove(kupovina);
        _storage.Save(_kupovine);
        return kupovina;
    }

    // Poziva se pri brisanju posetioca (referencijalna zavisnost)
    public void RemoveKupovineByPosetilac(int posetilacId)
    {
        _kupovine.RemoveAll(k => k.PosetilacId == posetilacId);
        _storage.Save(_kupovine);
    }

    public Kupovina? GetKupovinaById(int id)
    {
        return _kupovine.Find(k => k.Id == id);
    }

    public List<Kupovina> GetAllKupovine()
    {
        return _kupovine;
    }
}
