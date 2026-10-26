using System.Text.Json;
using System.Xml.Serialization;
namespace OOPLab3.DAL.Storage;
//Зберігає та читає колекції сутностей у форматі JSON
public class JsonDataProvider<T> : IDataProvider<T> where T : class, IFileRecord, new()
{
    //Серіалізує колекцію в JSON-файл
    public void Save(string path, IList<T> items)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(items, items.GetType(), options);
        File.WriteAllText(path, json);
    }
    //Зчитує JSON-файл як масив або список
    public IList<T> Load(string path, bool asArray)
    {
        string json = File.ReadAllText(path);
        if (asArray)
            return JsonSerializer.Deserialize<T[]>(json) ?? throw new InvalidDataException("Очікувався масив.");
        return JsonSerializer.Deserialize<List<T>>(json) ?? throw new InvalidDataException("Очікувався список.");
    }
}
//Зберігає та читає колекції сутностей у форматі XML
public class XmlDataProvider<T> : IDataProvider<T> where T : class, IFileRecord, new()
{
    //Серіалізує колекцію в XML-файл
    public void Save(string path, IList<T> items)
    {
        var serializer = new XmlSerializer(items.GetType());
        using var stream = File.Create(path);
        serializer.Serialize(stream, items);
    }
    //Десеріалізує XML-файл у масив або список
    public IList<T> Load(string path, bool asArray)
    {
        Type type = asArray ? typeof(T[]) : typeof(List<T>);
        var serializer = new XmlSerializer(type);
        using var stream = File.OpenRead(path);
        return (IList<T>)(serializer.Deserialize(stream) ?? throw new InvalidDataException("Порожній XML."));
    }
}
//У файлі: заголовок, кількість записів і поля кожної сутності у визначеному порядку
//Зберігає поля сутностей у власній бінарній послідовності
public class BinaryDataProvider<T> : IDataProvider<T> where T : class, IFileRecord, new()
{
    private const string Header = "OOPLAB3-BINARY-2";
    //Записує заголовок, кількість і бінарні поля сутностей
    public void Save(string path, IList<T> items)
    {
        using var writer = new BinaryWriter(File.Create(path));
        writer.Write(Header);
        writer.Write(items.Count);
        foreach (T item in items)
            item.WriteBinary(writer);
    }
    //Відновлює сутності з бінарного файлу та перевіряє його цілісність
    public IList<T> Load(string path, bool asArray)
    {
        using var reader = new BinaryReader(File.OpenRead(path));
        if (reader.ReadString() != Header)
            throw new InvalidDataException("Невідомий бінарний формат. Створіть новий файл.");
        int count = reader.ReadInt32();
        if (count < 0 || count > 100000)
            throw new InvalidDataException("Некоректна кількість записів.");
        IList<T> result;
        if (asArray) result = new T[count];
        else result = new List<T>();
        for (int i = 0; i < count; i++)
        {
            T item = new T();
            item.ReadBinary(reader);
            if (asArray) result[i] = item;
            else result.Add(item);
        }
        if (reader.BaseStream.Position != reader.BaseStream.Length)
            throw new InvalidDataException("Зайві дані в кінці файлу.");
        return result;
    }
}
public class CustomDataProvider<T> : IDataProvider<T> where T : class, IFileRecord, new()
{
    private const string Header = "OOPLAB3-CUSTOM-2";
    //Записує заголовок, кількість і текстове подання кожного запису
    public void Save(string path, IList<T> items)
    {
        using var writer = new StreamWriter(path);
        writer.WriteLine(Header);
        writer.WriteLine(items.Count);
        foreach (T item in items)
            writer.WriteLine(item.ToText());
    }
    //Відновлює записи з власного формату та перевіряє кількість рядків
    public IList<T> Load(string path, bool asArray)
    {
        using var reader = new StreamReader(path);
        if (reader.ReadLine() != Header)
            throw new InvalidDataException("Невідомий власний формат. Створіть новий файл.");
        if (!int.TryParse(reader.ReadLine(), out int count) || count < 0 || count > 100000)
            throw new InvalidDataException("Некоректна кількість записів.");
        IList<T> result;
        if (asArray) result = new T[count];
        else result = new List<T>();
        for (int i = 0; i < count; i++)
        {
            string line = reader.ReadLine() ?? throw new InvalidDataException("Файл обірваний.");
            T item = new T();
            item.FromText(line);
            if (asArray) result[i] = item;
            else result.Add(item);
        }
        if (reader.ReadLine() != null)
            throw new InvalidDataException("Зайві дані в кінці файлу.");
        return result;
    }
}