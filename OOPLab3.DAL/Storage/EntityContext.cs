namespace OOPLab3.DAL.Storage;
//Вибирає провайдер формату та надає спільні операції з файлом
public class EntityContext<T> where T : class, IFileRecord, new()
{
    private readonly IDataProvider<T> provider;
//Створює провайдер, що відповідає вибраному формату серіалізації
    public EntityContext(StorageFormat format)
    {
        switch (format)
        {
            case StorageFormat.Binary: provider = new BinaryDataProvider<T>(); break;
            case StorageFormat.Xml: provider = new XmlDataProvider<T>(); break;
            case StorageFormat.Json: provider = new JsonDataProvider<T>(); break;
            case StorageFormat.Custom: provider = new CustomDataProvider<T>(); break;
            default: throw new ArgumentException("Невідомий формат.");
        }
    }
//Готує каталог і делегує запис файлу відповідному провайдеру
    public void Save(string path, IList<T> items)
    {
        try
        {
            string? directory = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            provider.Save(path, items);
        }
        catch (Exception ex)
        {
            throw new DataAccessException("Не вдалося зберегти файл: " + path, ex);
        }
    }
    //Відсутній файл — це нова база. Пошкоджений файл не замінюється порожнім списком
//Завантажує записи; відсутній файл вважає новою порожньою базою
    public IList<T> Load(string path, bool asArray = false)
    {
        try
        {
            return provider.Load(path, asArray);
        }
        catch (FileNotFoundException)
        {
            if (asArray) return new T[0];
            return new List<T>();
        }
        catch (DirectoryNotFoundException)
        {
            if (asArray) return new T[0];
            return new List<T>();
        }
        catch (Exception ex)
        {
            throw new DataAccessException("Не вдалося прочитати файл: " + path, ex);
        }
    }
//Видаляє файл і перетворює системні помилки на помилку DAL
    public void Delete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception ex)
        {
            throw new DataAccessException("Не вдалося видалити файл: " + path, ex);
        }
    }
}
//Передає помилку доступу до файлу з початковою причиною
public class DataAccessException : Exception
{
//Створює помилку DAL, зберігаючи початковий виняток
    public DataAccessException(string message, Exception innerException) : base(message, innerException) { }
}