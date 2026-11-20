using SajamKnjiga.Serialization;
namespace SajamKnjiga.Model;

public class ListaZeljaStavka : ISerializable
{
    public int PosetilacId { get; set; }
    public int KnjigaId { get; set; }

    public ListaZeljaStavka() { }
    public ListaZeljaStavka(int posetilacId, int knjigaId) { PosetilacId = posetilacId; KnjigaId = knjigaId; }

    public string[] ToCSV() => new[] { PosetilacId.ToString(), KnjigaId.ToString() };
    public void FromCSV(string[] v) { PosetilacId = int.Parse(v[0]); KnjigaId = int.Parse(v[1]); }
}
