using System.Windows;
using System.Windows.Input;
using SajamKnjiga.DAO;

namespace SajamKnjigaWpf;

public partial class MainWindow : Window
{

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
    // Prave akcije dolaze u svojim tačkama, pa su za sada poruke.

    private void Add_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        // TODO 8.10: dijalog za dodavanje (posetilac / autor / knjiga)
        MessageBox.Show("Dodavanje entiteta (8.10)");
    }

    private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        // TODO 8.11: dijalog za izmjenu označenog entiteta
        MessageBox.Show("Izmjena entiteta (8.11)");
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
