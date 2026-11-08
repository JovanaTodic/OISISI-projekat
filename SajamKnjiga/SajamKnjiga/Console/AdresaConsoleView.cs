using SajamKnjiga.DAO;
using SajamKnjiga.Model;

namespace SajamKnjiga.Console;

/*
 * Preuzeto i prilagođeno iz CRUDExample (materijali sa vježbi).
 *
 * Klasa koju koristimo za interakciju sa korisnikom.
 * Ova klasa sadrži glavnu petlju menija, i učitava
 * i ispisuje podatke korisniku.
 * Ova klasa NE TREBA da sadrži poslovnu logiku!
 * Sva poslovna logika se nalazi u DAO klasi, a ova
 * klasa joj delegira posao i prosljeđuje potrebne podatke.
 */
class AdresaConsoleView
{
    private readonly AdresaDAO _adresaDao;

    public AdresaConsoleView(AdresaDAO adresaDao)
    {
        _adresaDao = adresaDao;
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
        System.Console.WriteLine("\nIzaberite opciju: ");
        System.Console.WriteLine("1: Prikaži sve adrese");
        System.Console.WriteLine("2: Dodaj adresu");
        System.Console.WriteLine("3: Izmijeni adresu");
        System.Console.WriteLine("4: Obriši adresu");
        System.Console.WriteLine("5: Prikaži i sortiraj adrese");
        System.Console.WriteLine("0: Zatvori");
    }

    private void HandleMenuInput(string input)
    {
        switch (input)
        {
            case "1":
                ShowAllAdrese();
                break;
            case "2":
                AddAdresa();
                break;
            case "3":
                UpdateAdresa();
                break;
            case "4":
                RemoveAdresa();
                break;
            case "5":
                ShowAndSortAdrese();
                break;
            default:
                System.Console.WriteLine("Nepoznata opcija.");
                break;
        }
    }

    private void ShowAllAdrese()
    {
        PrintAdrese(_adresaDao.GetAllAdrese());
    }

    private void PrintAdrese(List<Adresa> adrese)
    {
        System.Console.WriteLine("Adrese: ");
        string header = $"{"ID",3} | {"Ulica",-25} | {"Broj",-6} | {"Grad",-15} | Država";
        System.Console.WriteLine(header);
        foreach (Adresa a in adrese)
        {
            System.Console.WriteLine(a);
        }
    }

    private void AddAdresa()
    {
        Adresa adresa = InputAdresa();
        _adresaDao.AddAdresa(adresa);
        System.Console.WriteLine("Adresa je dodata.");
    }

    private Adresa InputAdresa()
    {
        System.Console.WriteLine("Unesite ulicu: ");
        string ulica = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite broj: ");
        string broj = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite grad: ");
        string grad = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite državu: ");
        string drzava = ConsoleViewUtils.SafeInputText();

        // Id se dodjeljuje u DAO klasi pri dodavanju
        return new Adresa(0, ulica, broj, grad, drzava);
    }

    private void UpdateAdresa()
    {
        int id = InputId();
        Adresa adresa = InputAdresa();
        adresa.Id = id;
        Adresa? updatedAdresa = _adresaDao.UpdateAdresa(adresa);
        if (updatedAdresa == null)
        {
            System.Console.WriteLine("Adresa nije pronađena.");
            return;
        }

        System.Console.WriteLine("Adresa je izmijenjena.");
    }

    private int InputId()
    {
        System.Console.WriteLine("Unesite id adrese: ");
        return ConsoleViewUtils.SafeInputInt();
    }

    private void RemoveAdresa()
    {
        int id = InputId();
        Adresa? removedAdresa = _adresaDao.RemoveAdresa(id);
        if (removedAdresa is null)
        {
            System.Console.WriteLine("Adresa nije pronađena.");
            return;
        }

        System.Console.WriteLine("Adresa je obrisana.");
    }

    private void ShowAndSortAdrese()
    {
        System.Console.WriteLine("\nUnesite stranicu: ");
        int page = ConsoleViewUtils.SafeInputInt();
        System.Console.WriteLine("\nUnesite broj adresa po stranici: ");
        int pageSize = ConsoleViewUtils.SafeInputInt();
        System.Console.WriteLine("\nUnesite kriterijum sortiranja (Id, Ulica, Broj, Grad, Drzava): ");
        string sortCriteria = System.Console.ReadLine() ?? string.Empty;
        System.Console.WriteLine("\nUnesite 0 za rastuće, bilo koji drugi broj za opadajuće: ");
        int sortDirectionInput = ConsoleViewUtils.SafeInputInt();
        SortDirection sortDirection = sortDirectionInput == 0 ? SortDirection.Ascending : SortDirection.Descending;

        PrintAdrese(_adresaDao.GetAllAdrese(page, pageSize, sortCriteria, sortDirection));
    }
}
