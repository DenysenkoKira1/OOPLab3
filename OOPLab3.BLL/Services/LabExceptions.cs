namespace OOPLab3.BLL.Services;
//Базовий виняток предметної області лабораторної
public abstract class LabException : Exception
{
//Створює виняток предметної області з поясненням
    protected LabException(string message) : base(message) { }
//Зберігає повідомлення та початкову причину помилки
    protected LabException(string message, Exception innerException) : base(message, innerException) { }
}
//Повідомляє про некоректно введені дані
public sealed class ValidationException : LabException
{
//Створює виняток для некоректних або неповних даних
    public ValidationException(string message) : base(message) { }
}
//Повідомляє, що потрібний запис не знайдено
public sealed class EntityNotFoundException : LabException
{
//Створює виняток, коли шуканого запису немає
    public EntityNotFoundException(string message) : base(message) { }
}
//Повідомляє про помилку під час читання або запису файлу
public sealed class StorageOperationException : LabException
{
//Створює виняток операції з файлом із початковою причиною
    public StorageOperationException(string message, Exception innerException) : base(message, innerException) { }
}