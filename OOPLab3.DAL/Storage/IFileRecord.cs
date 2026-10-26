namespace OOPLab3.DAL.Storage;
//Кожна сутність знає порядок своїх полів. Провайдер працює з усіма однаково
//Описує, як сутність записує та відновлює власні поля
public interface IFileRecord
{
//Записує поля сутності у бінарний потік у сталому порядку
    void WriteBinary(BinaryWriter writer);
//Зчитує поля сутності з бінарного потоку в тому самому порядку
    void ReadBinary(BinaryReader reader);
//Повертає текстове подання запису для власного формату
    string ToText();
//Відновлює поля сутності з одного рядка власного формату
    void FromText(string line);
}