namespace SajamKnjiga.Serialization;

public interface ISerializable
{
    string[] ToCSV();

    void FromCSV(string[] values);
}
