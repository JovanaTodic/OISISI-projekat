using System.Globalization;

namespace SajamKnjiga.Console;

// Preuzeto i prilagođeno iz CRUDExample (materijali sa vježbi)
static class ConsoleViewUtils
{
    public static int SafeInputInt()
    {
        int input;

        string rawInput = System.Console.ReadLine() ?? string.Empty;

        while (!int.TryParse(rawInput, out input))
        {
            System.Console.WriteLine("Nije ispravan broj, pokušajte ponovo: ");

            rawInput = System.Console.ReadLine() ?? string.Empty;
        }

        return input;
    }

    // Ceo broj u zadatom opsegu (uključujući granice)
    public static int SafeInputInt(int min, int max)
    {
        int value = SafeInputInt();

        while (value < min || value > max)
        {
            System.Console.WriteLine($"Unesite broj između {min} i {max}: ");
            value = SafeInputInt();
        }

        return value;
    }

    // Tekst ne smije biti prazan i ne smije sadržati '|' (to je razdvajač u fajlu).
    public static string SafeInputText()
    {
        string text = (System.Console.ReadLine() ?? string.Empty).Trim();

        while (text == string.Empty || text.Contains('|'))
        {
            System.Console.WriteLine("Polje ne smije biti prazno niti sadržati znak '|', pokušajte ponovo: ");

            text = (System.Console.ReadLine() ?? string.Empty).Trim();
        }

        return text;
    }

    // Datum u formatu 25.12.2025.
    public static DateTime SafeInputDate()
    {
        string[] formats = { "d.M.yyyy.", "d.M.yyyy" };
        DateTime date;

        string rawInput = (System.Console.ReadLine() ?? string.Empty).Trim();

        while (!DateTime.TryParseExact(rawInput, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
               || date > DateTime.Today)
        {
            System.Console.WriteLine("Nije ispravan datum (format 25.12.2025., ne smije biti u budućnosti), pokušajte ponovo: ");

            rawInput = (System.Console.ReadLine() ?? string.Empty).Trim();
        }

        return date;
    }

    // Nenegativan decimalni broj; dozvoljen je i zarez i tačka.
    public static decimal SafeInputDecimal()
    {
        decimal value;

        string rawInput = (System.Console.ReadLine() ?? string.Empty).Trim().Replace(',', '.');

        while (!decimal.TryParse(rawInput, NumberStyles.Number, CultureInfo.InvariantCulture, out value) || value < 0)
        {
            System.Console.WriteLine("Nije ispravan iznos, pokušajte ponovo: ");

            rawInput = (System.Console.ReadLine() ?? string.Empty).Trim().Replace(',', '.');
        }

        return value;
    }
}
