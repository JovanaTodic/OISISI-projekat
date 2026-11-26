using System.Windows;
using SajamKnjiga.Model;

namespace SajamKnjigaWpf;

// Dijalog za dodavanje autora (8.10, #dodavanje_autora)
public partial class AutorDialog : Window
{
    // Autor unesen u dijalog; postavlja se tek nakon "Potvrdi"
    private readonly Autor? _autor;
    public Autor? NoviAutor { get; private set; }

    public AutorDialog()
    {
        InitializeComponent();
        dpDatumRodjenja.DisplayDateEnd = DateTime.Today;   // rođenje ne može biti u budućnosti
    }

    // Izmjena (8.11): polja se popune podacima izabranog autora
    public AutorDialog(Autor autor) : this()
    {
        _autor = autor;
        Title = "Izmena autora";

        txtIme.Text = autor.Ime;
        txtPrezime.Text = autor.Prezime;
        dpDatumRodjenja.SelectedDate = autor.DatumRodjenja;
        txtTelefon.Text = autor.Telefon;
        txtEmail.Text = autor.Email;
        txtBrojLicneKarte.Text = autor.BrojLicneKarte;
        txtGodineIskustva.Text = autor.GodineIskustva.ToString();
        txtUlica.Text = autor.Adresa.Ulica;
        txtBroj.Text = autor.Adresa.Broj;
        txtGrad.Text = autor.Adresa.Grad;
        txtDrzava.Text = autor.Adresa.Drzava;

        btnPotvrdi.IsEnabled = SvaPoljaValidna();
    }

    // Poziva se pri svakoj promjeni bilo kog polja
    private void Polje_Changed(object sender, EventArgs e)
    {
        btnPotvrdi.IsEnabled = SvaPoljaValidna();
    }

    private bool SvaPoljaValidna()
    {
        // '|' je razdvajač u fajlovima, pa ne smije u tekst
        string[] tekstovi =
        {
            txtIme.Text, txtPrezime.Text, txtTelefon.Text, txtEmail.Text,
            txtBrojLicneKarte.Text, txtUlica.Text, txtBroj.Text, txtGrad.Text, txtDrzava.Text
        };
        if (tekstovi.Any(t => string.IsNullOrWhiteSpace(t) || t.Contains('|')))
            return false;

        if (dpDatumRodjenja.SelectedDate is null) return false;
        if (!txtEmail.Text.Contains('@')) return false;
        if (!int.TryParse(txtGodineIskustva.Text, out int godine) || godine < 0) return false;

        return true;
    }

    private void Potvrdi_Click(object sender, RoutedEventArgs e)
    {
        Adresa adresa = new Adresa(0, txtUlica.Text.Trim(), txtBroj.Text.Trim(),
                                   txtGrad.Text.Trim(), txtDrzava.Text.Trim());

        NoviAutor = new Autor(0, txtIme.Text.Trim(), txtPrezime.Text.Trim(),
                              dpDatumRodjenja.SelectedDate!.Value, adresa,
                              txtTelefon.Text.Trim(), txtEmail.Text.Trim(),
                              txtBrojLicneKarte.Text.Trim(), int.Parse(txtGodineIskustva.Text));

        if (_autor != null) NoviAutor.Id = _autor.Id;   // pri izmjeni autor zadržava svoj Id

        DialogResult = true;
    }
}
