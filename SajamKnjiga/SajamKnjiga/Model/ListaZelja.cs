using SajamKnjiga.Serialization;
namespace SajamKnjiga.Model;

public class ListaZeljaStavka : ISerializable
{
    public int PosetilacId { get; set; }
    public string Isbn { get; set; } = "";

    public ListaZeljaStavka() { }
    public ListaZeljaStavka(int posetilacId, string isbn) { PosetilacId = posetilacId; Isbn = isbn; }

    public string[] ToCSV() => new[] { PosetilacId.ToString(), Isbn };
    public void FromCSV(string[] v) { PosetilacId = int.Parse(v[0]); Isbn = v[1]; }
}
