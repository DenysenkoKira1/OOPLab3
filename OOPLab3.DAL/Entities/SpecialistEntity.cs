using System.Globalization;
using OOPLab3.DAL.Storage;
namespace OOPLab3.DAL.Entities;
//Сутність спеціаліста рівня DAL для зберігання у різних форматах
public class SpecialistEntity : IFileRecord
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

//Записує тип професії та ім’я у бінарний потік
    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Type);
        writer.Write(Name);
    }
//Відновлює тип професії та ім’я з бінарного потоку
    public void ReadBinary(BinaryReader reader)
    {
        Type = reader.ReadString();
        Name = reader.ReadString();
    }
    public string ToText()
    {
        return string.Join("|",
            Uri.EscapeDataString(Type),
            Uri.EscapeDataString(Name));
    }
//Перевіряє кількість полів і відновлює запис спеціаліста
    public void FromText(string line)
    {
        string[] fields = line.Split('|');
        if (fields.Length != 2)
            throw new InvalidDataException("Неправильна кількість полів у записі.");
        Type = Uri.UnescapeDataString(fields[0]);
        Name = Uri.UnescapeDataString(fields[1]);
    }
}