using System.Text;
using SajamKnjiga.Console;
using SajamKnjiga.DAO;

namespace SajamKnjiga;

class Program
{
    static void Main()
    {
        // da bi se slova č, ć, š, ž, đ ispravno prikazivala i unosila
        System.Console.OutputEncoding = Encoding.UTF8;
        System.Console.InputEncoding = Encoding.UTF8;

        // DAO objekti se prave samo jednom i dele se izmedju pogleda
        AutorDAO autorDao = new AutorDAO();
        KnjigaDAO knjigaDao = new KnjigaDAO();
        KupovinaDAO kupovinaDao = new KupovinaDAO();
        PosetilacDAO posetilacDao = new PosetilacDAO(kupovinaDao);
        IzdavacDAO izdavacDao = new IzdavacDAO(autorDao);

        AdresaConsoleView adresaView = new AdresaConsoleView(new AdresaDAO());
        AutorConsoleView autorView = new AutorConsoleView(autorDao);
        KnjigaConsoleView knjigaView = new KnjigaConsoleView(knjigaDao);
        PosetilacConsoleView posetilacView = new PosetilacConsoleView(posetilacDao);
        KupovinaConsoleView kupovinaView = new KupovinaConsoleView(kupovinaDao, posetilacDao, knjigaDao);
        IzdavacConsoleView izdavacView = new IzdavacConsoleView(izdavacDao, autorDao);

        while (true)
        {
            System.Console.WriteLine("\nSajam knjiga - glavni meni: ");
            System.Console.WriteLine("1: Adrese");
            System.Console.WriteLine("2: Autori");
            System.Console.WriteLine("3: Knjige");
            System.Console.WriteLine("4: Posetioci");
            System.Console.WriteLine("5: Kupovine");
            System.Console.WriteLine("6: Izdavači");
            System.Console.WriteLine("0: Izlaz");

            string input = System.Console.ReadLine() ?? "0";
            if (input == "0") break;

            switch (input)
            {
                case "1":
                    adresaView.RunMenu();
                    break;
                case "2":
                    autorView.RunMenu();
                    break;
                case "3":
                    knjigaView.RunMenu();
                    break;
                case "4":
                    posetilacView.RunMenu();
                    break;
                case "5":
                    kupovinaView.RunMenu();
                    break;
                case "6":
                    izdavacView.RunMenu();
                    break;
                default:
                    System.Console.WriteLine("Nepoznata opcija.");
                    break;
            }
        }
    }
}