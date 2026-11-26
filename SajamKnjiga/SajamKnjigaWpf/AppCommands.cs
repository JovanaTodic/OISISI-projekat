using System.Windows.Input;

namespace SajamKnjigaWpf;

// Komande sa akceleratorima. Iste komande će kasnije koristiti i Menu Bar (8.6).
public static class AppCommands
{
    public static readonly RoutedUICommand Add = new("Dodaj", "Add", typeof(AppCommands),
        new InputGestureCollection { new KeyGesture(Key.N, ModifierKeys.Control) });

    public static readonly RoutedUICommand Edit = new("Izmijeni", "Edit", typeof(AppCommands),
        new InputGestureCollection { new KeyGesture(Key.E, ModifierKeys.Control) });

    public static readonly RoutedUICommand Delete = new("Obriši", "Delete", typeof(AppCommands),
        new InputGestureCollection { new KeyGesture(Key.Delete) });

    public static readonly RoutedUICommand Search = new("Pretraži", "Search", typeof(AppCommands),
        new InputGestureCollection { new KeyGesture(Key.F, ModifierKeys.Control) });
}