using System.Windows;
using System.Windows.Input;
using SajamKnjiga.DAO;
using SajamKnjiga.Model;

namespace SajamKnjigaWpf;

public partial class MainWindow : Window
{
    private AutorDAO? _autorDao;

    public MainWindow()
    {

        InitializeComponent();

        // 3/4 širine i visine ekrana (centriranje je u XAML-u)
        Width = SystemParameters.PrimaryScreenWidth * 0.75;
        Height = SystemParameters.PrimaryScreenHeight * 0.75;

        try
        {
            UcitajPodatke();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Greška pri učitavanju podataka: {e.Message}", "Greška",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UcitajPodatke()
    {
        AdresaDAO adresaDao = new AdresaDAO();
        AutorDAO autorDao = new AutorDAO(adresaDao);
        _autorDao = autorDao;
        IzdavacDAO izdavacDao = new IzdavacDAO(autorDao);
        KupovinaDAO kupovinaDao = new KupovinaDAO();
        KnjigaDAO knjigaDao = new KnjigaDAO(kupovinaDao, izdavacDao);
        PosetilacDAO posetilacDao = new PosetilacDAO(kupovinaDao, adresaDao);
        knjigaDao.OsveziKupovine();
        _ = new KnjigaAutorDAO(knjigaDao, autorDao);
        _ = new ListaZeljaDAO(posetilacDao, knjigaDao);

        dgPosetioci.ItemsSource = posetilacDao.GetAllPosetioci();
        dgAutori.ItemsSource = autorDao.GetAllAutori();
        dgKnjige.ItemsSource = knjigaDao.GetAllKnjige();
    }

    // ---------- Toolbar (8.7) ----------

    // 8.10: dodavanje entiteta aktivnog taba. Do sada je urađen samo #dodavanje_autora.
    private void Add_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (tabControl.SelectedContent == dgAutori)
        {
            DodajAutora();
        }
        else
        {
            // TODO 8.10: #dodavanje_posetioca i #dodavanje_knjige
            MessageBox.Show("Dodavanje za ovaj tab još nije implementirano (8.10)");
        }
    }

    // #dodavanje_autora
    private void DodajAutora()
    {
        if (_autorDao is null) return;   // podaci nisu učitani

        AutorDialog dialog = new AutorDialog() { Owner = this };  
        if (dialog.ShowDialog() != true || dialog.NoviAutor is null) return;

        try
        {
            Autor autor = _autorDao.AddAutor(dialog.NoviAutor);   // upisuje i u fajlove (Autor.txt, Adresa.txt)

            // osvježi tabelu autora i označi novododatog
            dgAutori.Items.Refresh();
            dgAutori.SelectedItem = autor;
            dgAutori.ScrollIntoView(autor);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška pri dodavanju autora: {ex.Message}", "Greška",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // 8.11: izmjena označenog entiteta aktivnog taba. Do sada je urađena samo #izmena_autora.
    private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (tabControl.SelectedContent == dgAutori)
        {
            IzmeniAutora();
        }
        else
        {
            // TODO 8.11: #izmena_posetioca i #izmena_knjige
            MessageBox.Show("Izmjena za ovaj tab još nije implementirana (8.11)");
        }
    }

    // #izmena_autora
    private void IzmeniAutora()
    {
        if (_autorDao is null) return;   // podaci nisu učitani

        if (dgAutori.SelectedItem is not Autor izabrani)
        {
            MessageBox.Show("Izaberite autora kojeg želite da izmijenite.", "Izmjena autora",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        AutorDialog dialog = new AutorDialog(izabrani) { Owner = this };
        if (dialog.ShowDialog() != true || dialog.NoviAutor is null) return;

        try
        {
            Autor? izmijenjen = _autorDao.UpdateAutor(dialog.NoviAutor);   // upisuje i u fajlove
            if (izmijenjen is null)
            {
                MessageBox.Show("Autor nije pronađen.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // osvježi tabelu autora
            dgAutori.Items.Refresh();
            dgAutori.SelectedItem = izmijenjen;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška pri izmjeni autora: {ex.Message}", "Greška",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        // TODO 8.12: brisanje označenog entiteta
        MessageBox.Show("Brisanje entiteta (8.12)");
    }

    private void Search_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        // TODO 8.19: pretraga tabele aktivnog taba po tekstu iz polja txtSearch
        MessageBox.Show($"Pretraga (8.19): {txtSearch.Text}");
    }


}
