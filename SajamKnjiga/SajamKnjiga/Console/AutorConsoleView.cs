using SajamKnjiga.DAO;
using SajamKnjiga.Model;

namespace SajamKnjiga.Console;

/*
 * Preuzeto i prilagođeno iz CRUDExample (materijali sa vježbi).
 *
 * Klasa za interakciju sa korisnikom. NE sadrži poslovnu logiku -
 * sva poslovna logika je u DAO klasi, a ova klasa joj delegira posao.
 */
class AutorConsoleView
{
    private readonly AutorDAO _autorDao;

    public AutorConsoleView(AutorDAO autorDao)
    {
        _autorDao = autorDao;
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
        System.Console.WriteLine("\nAutori - izaberite opciju: ");
        System.Console.WriteLine("1: Prikaži sve autore");
        System.Console.WriteLine("2: Dodaj autora");
        System.Console.WriteLine("3: Izmijeni autora");
        System.Console.WriteLine("4: Obriši autora");
        System.Console.WriteLine("5: Prikaži i sortiraj autore");
        System.Console.WriteLine("0: Nazad");
    }

    private void HandleMenuInput(string input)
    {
        switch (input)
        {
            case "1":
                PrintAutori(_autorDao.GetAllAutori());
                break;
            case "2":
                AddAutor();
                break;
            case "3":
                UpdateAutor();
                break;
            case "4":
                RemoveAutor();
                break;
            case "5":
                ShowAndSortAutori();
                break;
            default:
                System.Console.WriteLine("Nepoznata opcija.");
                break;
        }
    }

    private void PrintAutori(List<Autor> autori)
    {
        System.Console.WriteLine("Autori: ");
        foreach (Autor a in autori)
        {
            System.Console.WriteLine(a);
            System.Console.WriteLine($"      Telefon: {a.Telefon} | Adresa: {a.Adresa.Ulica} {a.Adresa.Broj}, {a.Adresa.Grad}, {a.Adresa.Drzava}");
        }
    }

    private void AddAutor()
    {
        Autor autor = InputAutor();
        _autorDao.AddAutor(autor);
        System.Console.WriteLine("Autor je dodat.");
    }

    private Autor InputAutor()
    {
        System.Console.WriteLine("Unesite ime: ");
        string ime = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite prezime: ");
        string prezime = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite datum rođenja (npr. 15.03.1970.): ");
        DateTime datumRodjenja = ConsoleViewUtils.SafeInputDate();

        System.Console.WriteLine("Unesite telefon: ");
        string telefon = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite e-mail: ");
        string email = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite broj lične karte: ");
        string brojLicneKarte = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite godine iskustva: ");
        int godineIskustva = ConsoleViewUtils.SafeInputInt(0, 100);

        System.Console.WriteLine("Adresa autora:");
        System.Console.WriteLine("Unesite ulicu: ");
        string ulica = ConsoleViewUtils.SafeInputText();
        System.Console.WriteLine("Unesite broj: ");
        string broj = ConsoleViewUtils.SafeInputText();
        System.Console.WriteLine("Unesite grad: ");
        string grad = ConsoleViewUtils.SafeInputText();
        System.Console.WriteLine("Unesite državu: ");
        string drzava = ConsoleViewUtils.SafeInputText();

        // Id autora se dodjeljuje u DAO klasi pri dodavanju
        Adresa adresa = new Adresa(0, ulica, broj, grad, drzava);
        return new Autor(0, ime, prezime, datumRodjenja, adresa, telefon, email, brojLicneKarte, godineIskustva);
    }

    private void UpdateAutor()
    {
        int id = InputId();
        Autor autor = InputAutor();
        autor.Id = id;
        Autor? updatedAutor = _autorDao.UpdateAutor(autor);
        if (updatedAutor == null)
        {
            System.Console.WriteLine("Autor nije pronađen.");
            return;
        }

        System.Console.WriteLine("Autor je izmijenjen.");
    }

    private int InputId()
    {
        System.Console.WriteLine("Unesite id autora: ");
        return ConsoleViewUtils.SafeInputInt();
    }

    private void RemoveAutor()
    {
        int id = InputId();
        Autor? removedAutor = _autorDao.RemoveAutor(id);
        if (removedAutor is null)
        {
            System.Console.WriteLine("Autor nije pronađen.");
            return;
        }

        System.Console.WriteLine("Autor je obrisan.");
    }

    private void ShowAndSortAutori()
    {
        System.Console.WriteLine("\nUnesite stranicu: ");
        int page = ConsoleViewUtils.SafeInputInt();
        System.Console.WriteLine("\nUnesite broj autora po stranici: ");
        int pageSize = ConsoleViewUtils.SafeInputInt();
        System.Console.WriteLine("\nUnesite kriterijum sortiranja (Ime, Prezime, E_mejl): ");
        string sortCriteria = System.Console.ReadLine() ?? string.Empty;
        System.Console.WriteLine("\nUnesite 0 za rastuće, bilo koji drugi broj za opadajuće: ");
        int sortDirectionInput = ConsoleViewUtils.SafeInputInt();
        SortDirection sortDirection = sortDirectionInput == 0 ? SortDirection.Ascending : SortDirection.Descending;

        PrintAutori(_autorDao.GetAllAutori(page, pageSize, sortCriteria, sortDirection));
    }
}
