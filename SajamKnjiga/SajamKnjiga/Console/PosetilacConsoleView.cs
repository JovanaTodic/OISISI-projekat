using SajamKnjiga.DAO;
using SajamKnjiga.Model;

namespace SajamKnjiga.Console;

 
class PosetilacConsoleView
{
    private readonly PosetilacDAO _posetilacDao;

    public PosetilacConsoleView(PosetilacDAO posetilacDao)
    {
        _posetilacDao = posetilacDao;
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
        System.Console.WriteLine("\nPosetioci - izaberite opciju: ");
        System.Console.WriteLine("1: Prikaži sve posetioce");
        System.Console.WriteLine("2: Dodaj posetioca");
        System.Console.WriteLine("3: Izmijeni posetioca");
        System.Console.WriteLine("4: Obriši posetioca");
        System.Console.WriteLine("5: Prikaži i sortiraj posetioce");
        System.Console.WriteLine("0: Nazad");
    }

    private void HandleMenuInput(string input)
    {
        switch (input)
        {
            case "1":
                PrintPosetioci(_posetilacDao.GetAllPosetioci());
                break;
            case "2":
                AddPosetilac();
                break;
            case "3":
                UpdatePosetilac();
                break;
            case "4":
                RemovePosetilac();
                break;
            case "5":
                ShowAndSortPosetioci();
                break;
            default:
                System.Console.WriteLine("Nepoznata opcija.");
                break;
        }
    }

    private void PrintPosetioci(List<Posetilac> posetioci)
    {
        System.Console.WriteLine("Posetioci: ");
        foreach (Posetilac p in posetioci)
        {
            System.Console.WriteLine(p);
            System.Console.WriteLine($"      Adresa: {p.Adresa.Ulica} {p.Adresa.Broj}, {p.Adresa.Grad}, {p.Adresa.Drzava}");
        }
    }

    private void AddPosetilac()
    {
        Posetilac posetilac = InputPosetilac();
        Posetilac? dodat = _posetilacDao.AddPosetilac(posetilac);
        if (dodat is null)
        {
            System.Console.WriteLine("Posetilac nije dodat: broj članske karte već postoji.");
            return;
        }

        System.Console.WriteLine("Posetilac je dodat.");
    }

    private Posetilac InputPosetilac()
    {
        System.Console.WriteLine("Unesite broj članske karte (npr. CK-1-2023): ");
        string brojKarte = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite ime: ");
        string ime = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite prezime: ");
        string prezime = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite datum rođenja (npr. 15.03.1990.): ");
        DateTime datumRodjenja = ConsoleViewUtils.SafeInputDate();

        System.Console.WriteLine("Unesite telefon: ");
        string telefon = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite e-mail: ");
        string email = ConsoleViewUtils.SafeInputText();

        System.Console.WriteLine("Unesite godinu članstva: ");
        int godinaClanstva = ConsoleViewUtils.SafeInputInt(1900, DateTime.Today.Year);

        StatusPosetioca status = InputStatus();

        System.Console.WriteLine("Adresa posetioca:");
        System.Console.WriteLine("Unesite ulicu: ");
        string ulica = ConsoleViewUtils.SafeInputText();
        System.Console.WriteLine("Unesite broj: ");
        string broj = ConsoleViewUtils.SafeInputText();
        System.Console.WriteLine("Unesite grad: ");
        string grad = ConsoleViewUtils.SafeInputText();
        System.Console.WriteLine("Unesite državu: ");
        string drzava = ConsoleViewUtils.SafeInputText();

        // Id posetioca i adrese se dodjeljuje u DAO klasi
        Adresa adresa = new Adresa(0, ulica, broj, grad, drzava);
        return new Posetilac(0, ime, prezime, datumRodjenja, adresa, telefon, email, brojKarte, godinaClanstva, status);
    }

    private StatusPosetioca InputStatus()
    {
        System.Console.WriteLine("Unesite status (R - redovan, V - VIP): ");
        string input = (System.Console.ReadLine() ?? string.Empty).Trim().ToUpper();

        while (input != "R" && input != "V")
        {
            System.Console.WriteLine("Unesite R ili V: ");
            input = (System.Console.ReadLine() ?? string.Empty).Trim().ToUpper();
        }

        return input == "V" ? StatusPosetioca.Vip : StatusPosetioca.Redovan;
    }

    private void UpdatePosetilac()
    {
        int id = InputId();
        Posetilac posetilac = InputPosetilac();
        posetilac.Id = id;
        Posetilac? izmenjen = _posetilacDao.UpdatePosetilac(posetilac);
        if (izmenjen is null)
        {
            System.Console.WriteLine("Posetilac nije pronađen ili je broj članske karte zauzet.");
            return;
        }

        System.Console.WriteLine("Posetilac je izmijenjen.");
    }

    private int InputId()
    {
        System.Console.WriteLine("Unesite id posetioca: ");
        return ConsoleViewUtils.SafeInputInt();
    }

    private void RemovePosetilac()
    {
        int id = InputId();
        Posetilac? obrisan = _posetilacDao.RemovePosetilac(id);
        if (obrisan is null)
        {
            System.Console.WriteLine("Posetilac nije pronađen.");
            return;
        }

        System.Console.WriteLine("Posetilac je obrisan (zajedno sa njegovim kupovinama).");
    }

    private void ShowAndSortPosetioci()
    {
        System.Console.WriteLine("\nUnesite stranicu: ");
        int page = ConsoleViewUtils.SafeInputInt();
        System.Console.WriteLine("\nUnesite broj posetilaca po stranici: ");
        int pageSize = ConsoleViewUtils.SafeInputInt();
        System.Console.WriteLine("\nUnesite kriterijum sortiranja (BrojClanskeKarte, Ime, Prezime, GodinaClanstva, Status, ProsecnaOcena): ");
        string sortCriteria = System.Console.ReadLine() ?? string.Empty;
        System.Console.WriteLine("\nUnesite 0 za rastuće, bilo koji drugi broj za opadajuće: ");
        int sortDirectionInput = ConsoleViewUtils.SafeInputInt();
        SortDirection sortDirection = sortDirectionInput == 0 ? SortDirection.Ascending : SortDirection.Descending;

        PrintPosetioci(_posetilacDao.GetAllPosetioci(page, pageSize, sortCriteria, sortDirection));
    }
}
