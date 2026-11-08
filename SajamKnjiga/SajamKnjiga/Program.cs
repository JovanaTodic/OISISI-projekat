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

        AdresaConsoleView adresaView = new AdresaConsoleView(new AdresaDAO());
        AutorConsoleView autorView = new AutorConsoleView(new AutorDAO());
        KnjigaConsoleView knjigaView = new KnjigaConsoleView(new KnjigaDAO());

        while (true)
        {
            System.Console.WriteLine("\nSajam knjiga - glavni meni: ");
            System.Console.WriteLine("1: Adrese");
            System.Console.WriteLine("2: Autori");
            System.Console.WriteLine("3: Knjige");
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
                default:
                    System.Console.WriteLine("Nepoznata opcija.");
                    break;
            }
        }
    }
}
