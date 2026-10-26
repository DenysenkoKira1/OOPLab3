using OOPLab3.BLL.Models;
using OOPLab3.DAL.Entities;
using OOPLab3.DAL.Storage;
namespace OOPLab3.BLL.Services;
//Сервіс бізнес-логіки для студентів і спеціалістів; файли обробляє через EntityContext
public class EntityService
{
//Читає студентів із файлу та перетворює DAL-сутності на моделі BLL
    public List<StudentModel> GetStudents(string path, StorageFormat format)
    {
        try
        {
            var context = new EntityContext<StudentEntity>(format);
            var result = new List<StudentModel>();
            foreach (StudentEntity entity in context.Load(path))
            {
                if (entity == null)
                    throw new ValidationException("Файл містить порожній запис.");
                var model = new StudentModel
                {
                    Surname = entity.Surname,
                    FirstName = entity.FirstName,
                    Course = entity.Course,
                    StudentTicket = entity.StudentTicket,
                    ServedInArmy = entity.ServedInArmy,
                    MilitaryTicketNumber = entity.MilitaryTicketNumber,
                };
                Validate(model);
                result.Add(model);
            }
            return result;
        }
        catch (DataAccessException ex)
        {
            throw new StorageOperationException(ex.Message, ex);
        }
    }
//Перевіряє студентів, запобігає повторенню квитків і зберігає їх
    public void SaveStudents(string path, StorageFormat format, List<StudentModel> items)
    {
        var entities = new List<StudentEntity>();
        foreach (StudentModel model in items)
        {
            Validate(model);
            entities.Add(new StudentEntity
            {
                Surname = model.Surname,
                FirstName = model.FirstName,
                Course = model.Course,
                StudentTicket = model.StudentTicket,
                ServedInArmy = model.ServedInArmy,
                MilitaryTicketNumber = model.MilitaryTicketNumber,
            });
        }
        for (int i = 0; i < items.Count; i++)
            for (int j = i + 1; j < items.Count; j++)
                if (Same(items[i].StudentTicket, items[j].StudentTicket))
                    throw new ValidationException("Повторний номер студентського квитка.");
        try
        {
            var context = new EntityContext<StudentEntity>(format);
            context.Save(path, entities);
        }
        catch (DataAccessException ex)
        {
            throw new StorageOperationException(ex.Message, ex);
        }
    }
//Шукає студента за номером квитка або повідомляє, що запису немає
    public StudentModel FindStudent(string path, StorageFormat format, string ticket)
    {
        foreach (StudentModel item in GetStudents(path, format))
            if (Same(item.StudentTicket, ticket)) return item;
        throw new EntityNotFoundException("Запис не знайдено.");
    }
//Додає студента та зберігає оновлений список
    public void AddStudent(string path, StorageFormat format, StudentModel model)
    {
        List<StudentModel> items = GetStudents(path, format);
        items.Add(model);
        SaveStudents(path, format, items);
    }
//Видаляє студента за номером квитка та зберігає список
    public void RemoveStudent(string path, StorageFormat format, string ticket)
    {
        List<StudentModel> items = GetStudents(path, format);
        foreach (StudentModel item in items)
        {
            if (Same(item.StudentTicket, ticket))
            {
                items.Remove(item);
                SaveStudents(path, format, items);
                return;
            }
        }
        throw new EntityNotFoundException("Запис не знайдено.");
    }
//Видаляє файл зі студентами через рівень DAL
    public void DeleteStudentsFile(string path, StorageFormat format)
    {
        try
        {
            var context = new EntityContext<StudentEntity>(format);
            context.Delete(path);
        }
        catch (DataAccessException ex)
        {
            throw new StorageOperationException(ex.Message, ex);
        }
    }
//Читає спеціалістів і перетворює DAL-сутності на моделі BLL
    public List<SpecialistModel> GetSpecialists(string path, StorageFormat format)
    {
        try
        {
            var context = new EntityContext<SpecialistEntity>(format);
            var result = new List<SpecialistModel>();
            foreach (SpecialistEntity entity in context.Load(path))
            {
                if (entity == null)
                    throw new ValidationException("Файл містить порожній запис.");
                var model = new SpecialistModel
                {
                    Type = entity.Type,
                    Name = entity.Name,
                };
                Validate(model);
                result.Add(model);
            }
            return result;
        }
        catch (DataAccessException ex)
        {
            throw new StorageOperationException(ex.Message, ex);
        }
    }
//Перевіряє спеціалістів і зберігає їх без дублікатів
    public void SaveSpecialists(string path, StorageFormat format, List<SpecialistModel> items)
    {
        var entities = new List<SpecialistEntity>();
        foreach (SpecialistModel model in items)
        {
            Validate(model);
            entities.Add(new SpecialistEntity
            {
                Type = model.Type,
                Name = model.Name,
            });
        }
        for (int i = 0; i < items.Count; i++)
            for (int j = i + 1; j < items.Count; j++)
                if (items[i].Type == items[j].Type && Same(items[i].Name, items[j].Name))
                    throw new ValidationException("Повторний запис спеціаліста.");
        try
        {
            var context = new EntityContext<SpecialistEntity>(format);
            context.Save(path, entities);
        }
        catch (DataAccessException ex)
        {
            throw new StorageOperationException(ex.Message, ex);
        }
    }
//Шукає спеціаліста за професією та ім’ям
    public SpecialistModel FindSpecialist(string path, StorageFormat format, string type, string name)
    {
        foreach (SpecialistModel item in GetSpecialists(path, format))
            if (item.Type == type && Same(item.Name, name)) return item;
        throw new EntityNotFoundException("Запис не знайдено.");
    }
//Додає спеціаліста та зберігає оновлений список
    public void AddSpecialist(string path, StorageFormat format, SpecialistModel model)
    {
        List<SpecialistModel> items = GetSpecialists(path, format);
        items.Add(model);
        SaveSpecialists(path, format, items);
    }
//Видаляє спеціаліста за професією та ім’ям
    public void RemoveSpecialist(string path, StorageFormat format, string type, string name)
    {
        List<SpecialistModel> items = GetSpecialists(path, format);
        foreach (SpecialistModel item in items)
        {
            if (item.Type == type && Same(item.Name, name))
            {
                items.Remove(item);
                SaveSpecialists(path, format, items);
                return;
            }
        }
        throw new EntityNotFoundException("Запис не знайдено.");
    }
//Видаляє файл зі спеціалістами через рівень DAL
    public void DeleteSpecialistsFile(string path, StorageFormat format)
    {
        try
        {
            var context = new EntityContext<SpecialistEntity>(format);
            context.Delete(path);
        }
        catch (DataAccessException ex)
        {
            throw new StorageOperationException(ex.Message, ex);
        }
    }
//Повертає кількість студентів п’ятого курсу, які служили в армії
    public int CountFifthYearStudentsWhoServed(string path, StorageFormat format)
    {
        int count = 0;
        foreach (StudentModel student in GetStudents(path, format))
            if (student.Course == 5 && student.ServedInArmy) count++;
        return count;
    }
//Знаходить спеціаліста й повертає результат його вміння декламувати
    public string RecitePoetry(string path, StorageFormat format, string type, string name)
    {
        SpecialistModel specialist = FindSpecialist(path, format, type, name);
        return specialist.RecitePoetry();
    }
//Порівнює текст без урахування великих і малих літер
    private static bool Same(string first, string second)
    {
        return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
    }
//Перевіряє обов’язкові поля студента та дані про військовий квито
    private static void Validate(StudentModel student)
    {
        if (string.IsNullOrWhiteSpace(student.Surname) || string.IsNullOrWhiteSpace(student.FirstName))
            throw new ValidationException("Ім’я та прізвище не можуть бути порожніми.");
        if (student.Course < 1)
            throw new ValidationException("Курс має бути додатним числом.");
        if (string.IsNullOrWhiteSpace(student.StudentTicket))
            throw new ValidationException("Номер студентського квитка обов’язковий.");
        if (student.ServedInArmy && string.IsNullOrWhiteSpace(student.MilitaryTicketNumber))
            throw new ValidationException("Для студента, який служив, потрібен номер військового квитка.");
        if (!student.ServedInArmy && !string.IsNullOrWhiteSpace(student.MilitaryTicketNumber))
            throw new ValidationException("Номер військового квитка можна вказати лише для того, хто служив.");
    }
//Перевіряє допустиму професію та заповнене ім’я спеціаліста
    private static void Validate(SpecialistModel specialist)
    {
        if (specialist.Type != "FootballPlayer" && specialist.Type != "Lawyer")
            throw new ValidationException("Тип має бути FootballPlayer або Lawyer.");
        if (string.IsNullOrWhiteSpace(specialist.Name))
            throw new ValidationException("Ім’я не може бути порожнім.");
    }
}