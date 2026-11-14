using SajamKnjiga.DAO;
using SajamKnjiga.Model;

namespace SajamKnjiga.Console;

/*
 * Klasa za interakciju sa korisnikom. Ne sadrzi poslovnu logiku -
 * sva poslovna logika je u DAO klasi
 */
class IzdavacConsoleView
{
    private readonly IzdavacDAO _izdavacDao;
    private readonly AutorDAO _autorDao;

    public IzdavacConsoleView(IzdavacDAO izdavacDao, AutorDAO autorDao)
    {
        _izdavacDao = izdavacDao;
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
        System.Console.WriteLine("\nIzdavači - izaberite opciju: ");
        System.Console.WriteLine("1: Prikaži sve izdavače");
        System.Console.WriteLine("2: Dodaj izdavača");
        System.Console.WriteLine("3: Izmijeni izdavača");
        System.Console.WriteLine("4: Obriši izdavača");
        System.Console.WriteLine("5: Postavi šefa izdavača");
        System.Console.WriteLine("0: Nazad");
    }

    private void HandleMenuInput(string input)
    {
        switch (input)
        {
            case "1":
                PrintIzdavaci(_izdavacDao.GetAllIzdavaci());
                break;
            case "2":
                AddIzdavac();
                break;
            case "3":
                UpdateIzdavac();
                break;
            case "4":
                RemoveIzdavac();
                break;
            case "5":
                SetSef();
                break;
            default:
                System.Console.WriteLine("Nepoznata opcija.");
                break;
        }
    }

    private void PrintIzdavaci(List<Izdavac> izdavaci)
    {
        System.Console.WriteLine("Izdavači: ");
        foreach (Izdavac i in izdavaci)
        {
            System.Console.WriteLine(i);
        }
    }

    private void AddIzdavac()
    {
        System.Console.WriteLine("Unesite naziv izdavača: ");
        string naziv = ConsoleViewUtils.SafeInputText();

        // Sifra se dodjeljuje u DAO klasi pri dodavanju
        _izdavacDao.AddIzdavac(new Izdavac(0, naziv));
        System.Console.WriteLine("Izdavač je dodat (šef se postavlja opcijom 5).");
    }

    private void UpdateIzdavac()
    {
        int sifra = InputSifra();
        System.Console.WriteLine("Unesite novi naziv izdavača: ");
        string naziv = ConsoleViewUtils.SafeInputText();

        Izdavac? izmenjen = _izdavacDao.UpdateIzdavac(new Izdavac(sifra, naziv));
        if (izmenjen is null)
        {
            System.Console.WriteLine("Izdavač nije pronađen.");
            return;
        }

        System.Console.WriteLine("Izdavač je izmijenjen.");
    }

    private int InputSifra()
    {
        System.Console.WriteLine("Unesite šifru izdavača: ");
        return ConsoleViewUtils.SafeInputInt();
    }

    private void RemoveIzdavac()
    {
        int sifra = InputSifra();
        Izdavac? obrisan = _izdavacDao.RemoveIzdavac(sifra);
        if (obrisan is null)
        {
            System.Console.WriteLine("Izdavač nije pronađen.");
            return;
        }

        System.Console.WriteLine("Izdavač je obrisan.");
    }

    private void SetSef()
    {
        Izdavac? izdavac = _izdavacDao.GetIzdavacBySifra(InputSifra());
        if (izdavac is null)
        {
            System.Console.WriteLine("Izdavač nije pronađen.");
            return;
        }

        System.Console.WriteLine("Unesite id autora koji postaje šef: ");
        int autorId = ConsoleViewUtils.SafeInputInt();
        Autor? autor = _autorDao.GetAllAutori().Find(a => a.Id == autorId);
        if (autor is null)
        {
            System.Console.WriteLine("Autor nije pronađen.");
            return;
        }

        if (!_izdavacDao.PostaviSefa(izdavac, autor))
        {
            System.Console.WriteLine($"Greška: šef mora imati najmanje {Izdavac.MinGodinaIskustvaSefa} godina iskustva.");
            return;
        }

        System.Console.WriteLine("Šef je postavljen.");
    }
}
