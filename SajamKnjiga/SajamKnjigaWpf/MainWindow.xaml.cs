using System.Windows;
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
}