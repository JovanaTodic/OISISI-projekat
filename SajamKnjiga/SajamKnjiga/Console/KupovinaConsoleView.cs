using SajamKnjiga.DAO;
using SajamKnjiga.Model;

namespace SajamKnjiga.Console;


class KupovinaConsoleView
{
    private readonly KupovinaDAO _kupovinaDao;
    private readonly PosetilacDAO _posetilacDao;
    private readonly KnjigaDAO _knjigaDao;

    public KupovinaConsoleView(KupovinaDAO kupovinaDao, PosetilacDAO posetilacDao, KnjigaDAO knjigaDao)
    {
        _kupovinaDao = kupovinaDao;
        _posetilacDao = posetilacDao;
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
        System.Console.WriteLine("\nKupovine - izaberite opciju: ");
        System.Console.WriteLine("1: Prikaži sve kupovine");
        System.Console.WriteLine("2: Dodaj kupovinu");
        System.Console.WriteLine("3: Izmijeni kupovinu");
        System.Console.WriteLine("4: Obriši (poništi) kupovinu");
        System.Console.WriteLine("0: Nazad");
    }

    private void HandleMenuInput(string input)
    {
        switch (input)
        {
            case "1":
                PrintKupovine(_kupovinaDao.GetAllKupovine());
                break;
            case "2":
                AddKupovina();
                break;
            case "3":
                UpdateKupovina();
                break;
            case "4":
                RemoveKupovina();
                break;
            default:
                System.Console.WriteLine("Nepoznata opcija.");
                break;
        }
    }

    private void PrintKupovine(List<Kupovina> kupovine)
    {
        System.Console.WriteLine("Kupovine: ");
        foreach (Kupovina k in kupovine)
        {
            System.Console.WriteLine(k);
        }
    }

    private void AddKupovina()
    {
        System.Console.WriteLine("Unesite id posetioca: ");
        int posetilacId = ConsoleViewUtils.SafeInputInt();
        if (_posetilacDao.GetPosetilacById(posetilacId) is null)
        {
            System.Console.WriteLine("Posetilac nije pronađen.");
            return;
        }

        System.Console.WriteLine("Unesite ISBN knjige: ");
        string isbn = ConsoleViewUtils.SafeInputText();
        if (_knjigaDao.GetAllKnjige().Find(k => k.Isbn == isbn) is null)
        {
            System.Console.WriteLine("Knjiga nije pronađena.");
            return;
        }

        Kupovina kupovina = new Kupovina();
        kupovina.PosetilacId = posetilacId;
        kupovina.KnjigaIsbn = isbn;
        InputPodaciKupovine(kupovina);

        Kupovina? dodata = _kupovinaDao.AddKupovina(kupovina);
        if (dodata is null)
        {
            System.Console.WriteLine("Kupovina nije dodata: posetilac je već kupio tu knjigu.");
            return;
        }

        _posetilacDao.OsveziKupovine();
        System.Console.WriteLine("Kupovina je dodata.");
    }

    // datum, ocena i komentar (zajednicko za dodavanje i izmenu)
    private void InputPodaciKupovine(Kupovina kupovina)
    {
        System.Console.WriteLine("Unesite datum kupovine (npr. 15.03.2025.): ");
        kupovina.DatumKupovine = ConsoleViewUtils.SafeInputDate();

        System.Console.WriteLine("Unesite ocenu (1-5): ");
        kupovina.Ocena = ConsoleViewUtils.SafeInputInt(1, 5);

        System.Console.WriteLine("Unesite komentar: ");
        kupovina.Komentar = ConsoleViewUtils.SafeInputText();
    }

    private void UpdateKupovina()
    {
        int id = InputId();
        Kupovina kupovina = new Kupovina();
        kupovina.Id = id;
        InputPodaciKupovine(kupovina);

        Kupovina? izmenjena = _kupovinaDao.UpdateKupovina(kupovina);
        if (izmenjena is null)
        {
            System.Console.WriteLine("Kupovina nije pronađena.");
            return;
        }

        _posetilacDao.OsveziKupovine();
        System.Console.WriteLine("Kupovina je izmijenjena.");
    }

    private int InputId()
    {
        System.Console.WriteLine("Unesite id kupovine: ");
        return ConsoleViewUtils.SafeInputInt();
    }

    private void RemoveKupovina()
    {
        int id = InputId();
        Kupovina? obrisana = _kupovinaDao.RemoveKupovina(id);
        if (obrisana is null)
        {
            System.Console.WriteLine("Kupovina nije pronađena.");
            return;
        }

        _posetilacDao.OsveziKupovine();
        System.Console.WriteLine("Kupovina je obrisana.");
    }
}
