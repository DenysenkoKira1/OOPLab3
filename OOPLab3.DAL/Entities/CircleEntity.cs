using System.Globalization;
using OOPLab3.DAL.Storage;
namespace OOPLab3.DAL.Entities;
//Сутність кола рівня DAL, що відповідає за формати його запису
public class CircleEntity : IFileRecord
{
    public string FillColor { get; set; } = string.Empty;
    public string OutlineColor { get; set; } = string.Empty;
    public double Radius { get; set; }
//Записує кольори та радіус кола у бінарному форматі
    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(FillColor);
        writer.Write(OutlineColor);
        writer.Write(Radius);
    }
//Відновлює кольори та радіус кола з бінарного потоку
    public void ReadBinary(BinaryReader reader)
    {
        FillColor = reader.ReadString();
        OutlineColor = reader.ReadString();
        Radius = reader.ReadDouble();
    }
    public string ToText()
    {
        return string.Join("|",
            Uri.EscapeDataString(FillColor),
            Uri.EscapeDataString(OutlineColor),
            Radius.ToString(CultureInfo.InvariantCulture));
    }
//Розбирає рядок власного формату та відновлює поля кола
    public void FromText(string line)
    {
        string[] fields = line.Split('|');
        if (fields.Length != 3)
            throw new InvalidDataException("Неправильна кількість полів у записі.");
        FillColor = Uri.UnescapeDataString(fields[0]);
        OutlineColor = Uri.UnescapeDataString(fields[1]);
        Radius = double.Parse(fields[2], CultureInfo.InvariantCulture);
    }
}