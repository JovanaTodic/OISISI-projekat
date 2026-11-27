using System.Windows;
using System.Windows.Input;
using SajamKnjiga.DAO;
using SajamKnjiga.Model;

namespace SajamKnjigaWpf;

public partial class MainWindow : Window
{
    private AutorDAO? _autorDao;
    private IzdavacDAO? _izdavacDao;
    private KnjigaDAO? _knjigaDao;
    private PosetilacDAO? _posetilacDao;
    private KnjigaAutorDAO? _knjigaAutorDao;
    private ListaZeljaDAO? _listaZeljaDao;

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
        _knjigaAutorDao = new KnjigaAutorDAO(knjigaDao, autorDao);
        _listaZeljaDao = new ListaZeljaDAO(posetilacDao, knjigaDao);

        dgPosetioci.ItemsSource = posetilacDao.GetAllPosetioci();
        dgAutori.ItemsSource = autorDao.GetAllAutori();
        dgKnjige.ItemsSource = knjigaDao.GetAllKnjige();

        _izdavacDao = izdavacDao;
        _knjigaDao = knjigaDao;
        _posetilacDao = posetilacDao;
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

    // 8.12: brisanje označenog entiteta aktivnog taba. Urađeni su #brisanje_autora i #brisanje_knjige.
    private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (tabControl.SelectedContent == dgAutori)
        {
            ObrisiAutora();
        }
        else if (tabControl.SelectedContent == dgKnjige)
        {
            ObrisiKnjigu();
        }
        else
        {
            // TODO 8.12: #brisanje_posetioca
            MessageBox.Show("Brisanje posetioca još nije implementirano (8.12)");
        }
    }

    // #brisanje_autora
    private void ObrisiAutora()
    {
        if (_autorDao is null || _izdavacDao is null || _knjigaAutorDao is null) return;   // podaci nisu učitani

        if (dgAutori.SelectedItem is not Autor autor)
        {
            MessageBox.Show("Izaberite autora kojeg želite da obrišete.", "Brisanje autora",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        // referencijalna zavisnost: autor koji je jedini autor neke knjige ne može se obrisati
        if (_autorDao.JeJedinAutorNekeKnjige(autor))
        {
            MessageBox.Show("Autor je jedini autor bar jedne knjige i ne može se obrisati.", "Brisanje autora",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show("Da li ste sigurni da želite da obrišete autora?", "Brisanje autora",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        try
        {
            Autor? obrisan = _autorDao.RemoveAutor(autor.Id);
            if (obrisan is null) return;

            _izdavacDao.UkloniSefa(obrisan.Id);          // ako je bio šef izdavača, izdavač ostaje bez šefa
            _knjigaAutorDao.UkloniAutoraSvuda(obrisan);  // ukloni autora sa svih knjiga i osvježi fajl veza

            dgAutori.Items.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška pri brisanju autora: {ex.Message}", "Greška",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // #brisanje_knjige
    private void ObrisiKnjigu()
    {
        if (_knjigaDao is null || _posetilacDao is null || _listaZeljaDao is null || _knjigaAutorDao is null) return;

        if (dgKnjige.SelectedItem is not Knjiga knjiga)
        {
            MessageBox.Show("Izaberite knjigu koju želite da obrišete.", "Brisanje knjige",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (MessageBox.Show("Da li ste sigurni da želite da obrišete knjigu? Biće obrisane i sve njene kupovine.",
                "Brisanje knjige", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        try
        {
            Knjiga? obrisana = _knjigaDao.RemoveKnjiga(knjiga.Id);   // briše i kupovine te knjige
            if (obrisana is null) return;

            _posetilacDao.OsveziKupovine();                 // kupovine su obrisane, osvježi liste kupovina posetilaca
            _listaZeljaDao.UkloniSveKnjige(obrisana);       // ukloni knjigu sa svih listi želja
            _knjigaAutorDao.Sacuvaj();                      // osvježi fajl veza knjiga-autor

            dgKnjige.Items.Refresh();
            dgPosetioci.Items.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška pri brisanju knjige: {ex.Message}", "Greška",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Search_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        // TODO 8.19: pretraga tabele aktivnog taba po tekstu iz polja txtSearch
        MessageBox.Show($"Pretraga (8.19): {txtSearch.Text}");
    }


}
