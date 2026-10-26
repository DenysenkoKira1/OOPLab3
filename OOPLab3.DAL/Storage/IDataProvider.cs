namespace OOPLab3.DAL.Storage;
//IList<T> дозволяє передати і масив T[], і список List<T> без перетворення
//Спільний контракт провайдерів для різних форматів файлів
public interface IDataProvider<T> where T : class, IFileRecord, new()
{
//Зберігає колекцію сутностей у файл
    void Save(string path, IList<T> items);
//Завантажує сутності з файлу як масив або список
    IList<T> Load(string path, bool asArray);
}