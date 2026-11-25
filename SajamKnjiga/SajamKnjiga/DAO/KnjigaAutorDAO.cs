using SajamKnjiga.Model;
using SajamKnjiga.Storage;

namespace SajamKnjiga.DAO;

/*
 * Zaduzena za vezu vise-prema-vise izmedju knjiga i autora.
 * Cita i pise knjige_autori.txt, i povezuje objekte:
 * knjiga.Autori <-> autor.Knjige.
 */
public class KnjigaAutorDAO
{
    private readonly Storage<KnjigaAutor> _storage = new("knjige_autori.txt");
    private readonly KnjigaDAO _knjigaDao;

    public KnjigaAutorDAO(KnjigaDAO knjigaDao, AutorDAO autorDao)
    {
        _knjigaDao = knjigaDao;
        Povezi(knjigaDao.GetAllKnjige(), autorDao.GetAllAutori());
    }

    private void Povezi(List<Knjiga> knjige, List<Autor> autori)
    {
        foreach (KnjigaAutor veza in _storage.Load())
        {
            Knjiga? knjiga = knjige.FirstOrDefault(k => k.Id == veza.KnjigaId);
            Autor? autor = autori.FirstOrDefault(a => a.Id == veza.AutorId);

            // preskoci veze koje pokazuju na nepostojecu knjigu ili autora
            if (knjiga == null || autor == null)
                continue;

            if (!knjiga.Autori.Contains(autor))
                knjiga.Autori.Add(autor);

            if (!autor.Knjige.Contains(knjiga))
                autor.Knjige.Add(knjiga);
        }
    }

    public bool DodajAutora(Knjiga knjiga, Autor autor)
    {
        if (knjiga.Autori.Contains(autor)) return false;

        knjiga.Autori.Add(autor);
        autor.Knjige.Add(knjiga);
        Sacuvaj();
        return true;
    }

    // knjiga mora imati bar jednog autora
    public bool UkloniAutora(Knjiga knjiga, Autor autor)
    {
        if (!knjiga.Autori.Contains(autor) || knjiga.Autori.Count == 1) return false;

        knjiga.Autori.Remove(autor);
        autor.Knjige.Remove(knjiga);
        Sacuvaj();
        return true;
    }

    // pozvati nakon brisanja autora
    public void UkloniAutoraSvuda(Autor autor)
    {
        foreach (Knjiga k in autor.Knjige)
            k.Autori.Remove(autor);

        autor.Knjige.Clear();
        Sacuvaj();
    }

    // pozvati i nakon brisanja knjige
    public void Sacuvaj()
    {
        List<KnjigaAutor> veze = new();

        foreach (Knjiga knjiga in _knjigaDao.GetAllKnjige())
            foreach (Autor autor in knjiga.Autori)
                veze.Add(new KnjigaAutor(knjiga.Id, autor.Id));

        _storage.Save(veze);
    }
}

