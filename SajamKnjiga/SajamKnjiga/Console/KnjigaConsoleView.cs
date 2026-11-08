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

    public KnjigaConsoleView(KnjigaDAO knjigaDao)
    {
        _knjigaDao = knjigaDao;
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

        Knjiga knjiga = InputKnjiga(isbn);
        Knjiga? addedKnjiga = _knjigaDao.AddKnjiga(knjiga);
        if (addedKnjiga is null)
        {
            System.Console.WriteLine("Knjiga sa tim ISBN-om već postoji.");
            return;
        }

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

        return new Knjiga(isbn, naziv, zanr, godinaIzdanja, cena, brojStrana);
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

        System.Console.WriteLine("Knjiga je obrisana.");
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
}
