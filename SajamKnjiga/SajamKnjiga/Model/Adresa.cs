using SajamKnjiga.Serialization;

namespace SajamKnjiga.Model;

public class Adresa : ISerializable
{
    public int Id { get; set; }
    public string Ulica { get; set; }
    public string Broj { get; set; }
    public string Grad { get; set; }
    public string Drzava { get; set; }

    public Adresa()
    {
    }

    public Adresa(int id, string ulica, string broj, string grad, string drzava)
    {
        Id = id;
        Ulica = ulica;
        Broj = broj;
        Grad = grad;
        Drzava = drzava;
    }

    public override string ToString()
    {
        return $"{Id,3} | {Ulica,-25} | {Broj,-6} | {Grad,-15} | {Drzava}";
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            Ulica, 
            Broj, 
            Grad, 
            Drzava
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Ulica = values[1];
        Broj = values[2];
        Grad = values[3];
        Drzava = values[4];
    }



}
