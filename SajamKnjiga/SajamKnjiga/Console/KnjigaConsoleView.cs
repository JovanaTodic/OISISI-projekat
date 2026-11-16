using SajamKnjiga.DAO;
using SajamKnjiga.Model;

namespace SajamKnjiga.Console;

/*
 * Preuzeto i prilagođeno iz CRUDExample (materijali sa vježbi).
 *
 * Klasa za interakciju sa korisnikom. NE sadrži poslovnu logiku -
 * sva poslovna logika je u DAO klasi, a ova klasa joj delegira posao.
 */
class KnjigaConsoleView
{
    private readonly KnjigaDAO _knjigaDao;
    private readonly PosetilacDAO _posetilacDao;
    private readonly AutorDAO _autorDao;
    private readonly KnjigaAutorDAO _knjigaAutorDao;
    private readonly ListaZeljaDAO _listaZeljaDao;

    public KnjigaConsoleView(KnjigaDAO knjigaDao, PosetilacDAO posetilacDao, AutorDAO autorDao,
                             KnjigaAutorDAO knjigaAutorDao, ListaZeljaDAO listaZeljaDao)
    {
        _knjigaDao = knjigaDao;
        _posetilacDao = posetilacDao;
        _autorDao = autorDao;
        _knjigaAutorDao = knjigaAutorDao;
        _listaZeljaDao = listaZeljaDao;
    }

    public void RunMenu()
    {
        while (true)
        {
            ShowMenu();
            string userInput = System.Console.ReadLine() ?? "0";
            if (userInput == "0") break;
            HandleMenuInput(userInput);
        }
    }

    private void ShowMenu()
    {
        System.Console.WriteLine("\nKnjige - izaberite opciju: ");
        System.Console.WriteLine("1: Prikaži sve knjige");
        System.Console.WriteLine("2: Dodaj knjigu");
        System.Console.WriteLine("3: Izmijeni knjigu");
        System.Console.WriteLine("4: Obriši knjigu");
        System.Console.WriteLine("5: Prikaži i sortiraj knjige");
        System.Console.WriteLine("6: Dodaj knjigu na listu želja");
        System.Console.WriteLine("7: Ukloni knjigu sa liste želja");
        System.Console.WriteLine("8: Dodaj autora knjizi");
        System.Console.WriteLine("0: Nazad");
    }

    private void HandleMenuInput(string input)
    {
        switch (input)
        {
            case "1":
                PrintKnjige(_knjigaDao.GetAllKnjige());
                break;
            case "2":
                AddKnjiga();
                break;
            case "3":
                UpdateKnjiga();
                break;
            case "4":
                RemoveKnjiga();
                break;
            case "5":
                ShowAndSortKnjige();
                break;
            case "6": ListaZelja(true); break;
            case "7": ListaZelja(false); break;
            case "8": AddAutorKnjizi(); break;
            default:
                System.Console.WriteLine("Nepoznata opcija.");
                break;
        }
    }

    private void PrintKnjige(List<Knjiga> knjige)
    {
        System.Console.WriteLine("Knjige: ");
        System.Console.WriteLine("ISBN | Naziv | Žanr | Godina izdanja | Cena | Broj strana");
        foreach (Knjiga k in knjige)
        {
            System.Console.WriteLine(k);
        }
    }

    private void AddKnjiga()
    {
        System.Console.WriteLine("Unesite ISBN: ");
        string isbn = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite id autora (knjiga mora imati bar jednog): ");
        int autorId = ConsoleViewUtils.SafeInputInt();
        Autor? autor = _autorDao.GetAllAutori().Find(a => a.Id == autorId);
        if (autor is null)
        {
            System.Console.WriteLine("Autor nije pronađen.");
            return;
        }

        Knjiga knjiga = InputKnjiga(isbn);
        Knjiga? dodata = _knjigaDao.AddKnjiga(knjiga);
        if (dodata is null)
        {
            System.Console.WriteLine("ISBN već postoji ili izdavač sa tom šifrom ne postoji.");
            return;
        }

        _knjigaAutorDao.DodajAutora(dodata, autor);
        System.Console.WriteLine("Knjiga je dodata.");
    }
 

    // ISBN je ključ knjige, pa se pri izmjeni ne unosi ponovo.
    private Knjiga InputKnjiga(string isbn)
    {
        System.Console.WriteLine("Unesite naziv: ");
        string naziv = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite žanr: ");
        string zanr = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite godinu izdanja: ");
        int godinaIzdanja = ConsoleViewUtils.SafeInputInt(1450, DateTime.Today.Year);

        System.Console.WriteLine("Unesite cijenu: ");
        decimal cena = ConsoleViewUtils.SafeInputDecimal();

        System.Console.WriteLine("Unesite broj strana: ");
        int brojStrana = ConsoleViewUtils.SafeInputInt(1, 100000);

        System.Console.WriteLine("Unesite šifru izdavača: ");
        int izdavacId = ConsoleViewUtils.SafeInputInt();

        Knjiga knjiga = new Knjiga(isbn, naziv, zanr, godinaIzdanja, cena, brojStrana);
        knjiga.IzdavacId = izdavacId;
        return knjiga;
    }

    private void UpdateKnjiga()
    {
        System.Console.WriteLine("Unesite ISBN knjige koju mijenjate: ");
        string isbn = ConsoleViewUtils.SafeInputText();

        Knjiga knjiga = InputKnjiga(isbn);
        Knjiga? updatedKnjiga = _knjigaDao.UpdateKnjiga(knjiga);
        if (updatedKnjiga == null)
        {
            System.Console.WriteLine("Knjiga nije pronađena.");
            return;
        }

        System.Console.WriteLine("Knjiga je izmijenjena.");
    }

    private void RemoveKnjiga()
    {
        System.Console.WriteLine("Unesite ISBN knjige: ");
        string isbn = ConsoleViewUtils.SafeInputText();

        Knjiga? removedKnjiga = _knjigaDao.RemoveKnjiga(isbn);
        if (removedKnjiga is null)
        {
            System.Console.WriteLine("Knjiga nije pronađena.");
            return;
        }
        // kupovine obrisane knjige su uklonjene, pa osvjezi liste kupovina posetilaca
        _posetilacDao.OsveziKupovine();
        _listaZeljaDao.UkloniSveKnjige(removedKnjiga);
        _knjigaAutorDao.Sacuvaj();
        System.Console.WriteLine("Knjiga je obrisana (zajedno sa njenim kupovinama).");
    }

    private void ShowAndSortKnjige()
    {
        System.Console.WriteLine("\nUnesite stranicu: ");
        int page = ConsoleViewUtils.SafeInputInt();
        System.Console.WriteLine("\nUnesite broj knjiga po stranici: ");
        int pageSize = ConsoleViewUtils.SafeInputInt();
        System.Console.WriteLine("\nUnesite kriterijum sortiranja (Isbn, Naziv, Cena, GodinaIzdanja, Zanr): ");
        string sortCriteria = System.Console.ReadLine() ?? string.Empty;
        System.Console.WriteLine("\nUnesite 0 za rastuće, bilo koji drugi broj za opadajuće: ");
        int sortDirectionInput = ConsoleViewUtils.SafeInputInt();
        SortDirection sortDirection = sortDirectionInput == 0 ? SortDirection.Ascending : SortDirection.Descending;

        PrintKnjige(_knjigaDao.GetAllKnjige(page, pageSize, sortCriteria, sortDirection));
    }

    private void ListaZelja(bool dodaj)
    {
        System.Console.WriteLine("Unesite id posetioca: ");
        Posetilac? p = _posetilacDao.GetPosetilacById(ConsoleViewUtils.SafeInputInt());
        System.Console.WriteLine("Unesite ISBN knjige: ");
        Knjiga? k = _knjigaDao.GetKnjigaByIsbn(ConsoleViewUtils.SafeInputText());
        if (p is null || k is null)
        {
            System.Console.WriteLine("Posetilac ili knjiga nisu pronađeni.");
            return;
        }

        bool uspeh = dodaj ? _listaZeljaDao.Dodaj(p, k) : _listaZeljaDao.Ukloni(p, k);
        System.Console.WriteLine(uspeh ? "Gotovo." : "Nije promenjeno (već postoji ili je nema na listi).");
    }

    private void AddAutorKnjizi()
    {
        System.Console.WriteLine("Unesite ISBN knjige: ");
        Knjiga? k = _knjigaDao.GetKnjigaByIsbn(ConsoleViewUtils.SafeInputText());
        System.Console.WriteLine("Unesite id autora: ");
        int autorId = ConsoleViewUtils.SafeInputInt();
        Autor? a = _autorDao.GetAllAutori().Find(x => x.Id == autorId);
        if (k is null || a is null)
        {
            System.Console.WriteLine("Knjiga ili autor nisu pronađeni.");
            return;
        }

        System.Console.WriteLine(_knjigaAutorDao.DodajAutora(k, a) ? "Autor je dodat knjizi." : "Taj autor je već na knjizi.");
    }
}
