namespace SajamKnjiga.Serialization;

//Preuzeto iz CRUDExample (materijali sa vježbi)
public interface ISerializable
{
    string[] ToCSV();

    void FromCSV(string[] values);
}
