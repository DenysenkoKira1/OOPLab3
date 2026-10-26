using System.Globalization;
using OOPLab3.DAL.Storage;
namespace OOPLab3.DAL.Entities;
//Сутність студента рівня DAL для зберігання у різних форматах
public class StudentEntity : IFileRecord
{
    public string Surname { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public int Course { get; set; }
    public string StudentTicket { get; set; } = string.Empty;
    public bool ServedInArmy { get; set; }
    public string? MilitaryTicketNumber { get; set; }
//Записує поля студента у бінарному форматі та зберігає наявність квитка
    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Surname);
        writer.Write(FirstName);
        writer.Write(Course);
        writer.Write(StudentTicket);
        writer.Write(ServedInArmy);
        writer.Write(MilitaryTicketNumber != null);
        if (MilitaryTicketNumber != null) writer.Write(MilitaryTicketNumber);
    }
//Відновлює поля студента з бінарного потоку в тому самому порядку
    public void ReadBinary(BinaryReader reader)
    {
        Surname = reader.ReadString();
        FirstName = reader.ReadString();
        Course = reader.ReadInt32();
        StudentTicket = reader.ReadString();
        ServedInArmy = reader.ReadBoolean();
        MilitaryTicketNumber = reader.ReadBoolean() ? reader.ReadString() : null;
    }
    public string ToText()
    {
        return string.Join("|",
            Uri.EscapeDataString(Surname),
            Uri.EscapeDataString(FirstName),
            Course.ToString(CultureInfo.InvariantCulture),
            Uri.EscapeDataString(StudentTicket),
            ServedInArmy.ToString(),
            Uri.EscapeDataString(MilitaryTicketNumber ?? ""));
    }
//Перевіряє кількість полів і відновлює студента з текстового рядка
    public void FromText(string line)
    {
        string[] fields = line.Split('|');
        if (fields.Length != 6)
            throw new InvalidDataException("Неправильна кількість полів у записі.");
        Surname = Uri.UnescapeDataString(fields[0]);
        FirstName = Uri.UnescapeDataString(fields[1]);
        Course = int.Parse(fields[2], CultureInfo.InvariantCulture);
        StudentTicket = Uri.UnescapeDataString(fields[3]);
        ServedInArmy = bool.Parse(fields[4]);
        MilitaryTicketNumber = fields[5].Length == 0 ? null : Uri.UnescapeDataString(fields[5]);
    }
}