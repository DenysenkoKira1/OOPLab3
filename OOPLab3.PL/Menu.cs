using OOPLab3.BLL.Models;
using OOPLab3.BLL.Services;
using OOPLab3.DAL.Storage;
using OOPLab3.PL.Models;
namespace OOPLab3.PL;
//Показує консольне меню та передає введені дані сервісам бізнес-логіки
public class Menu
{
    private readonly CircleService circleService = new CircleService();
    private readonly EntityService entityService = new EntityService();
//Показує головне меню та запускає вибраний сценарій роботи
    public void MainMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Лабораторна робота 3.3 ---");
            Console.WriteLine("1 - Серіалізація кіл\n2 - Студенти\n3 - FootballPlayer і Lawyer\n0 - Вихід");
            try
            {
                switch (ReadLine())
                {
                    case "1": WorkWithCircles(); break;
                    case "2": WorkWithStudents(); break;
                    case "3": WorkWithSpecialists(); break;
                    case "0": return;
                    default: Console.WriteLine("Невідомий пункт."); break;
                }
            }
            catch (LabException ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
            }
            catch (EndOfStreamException)
            {
                return;
            }
        }
    }
//Демонструє збереження та відновлення масиву і списку кіл
    private void WorkWithCircles()
    {
        StorageFormat format = ReadFormat();
        string arrayPath = ReadFilePath("circles", format);
        CircleModel[] circles = circleService.CreateSixExamples();
        circleService.Save(arrayPath, format, circles);
        CircleModel[] restoredArray = circleService.LoadArray(arrayPath, format);
        Console.WriteLine("Відновлений масив:");
        PrintCircles(restoredArray);
        string listPath = ReadFilePath("circles-list", format);
        if (string.Equals(Path.GetFullPath(arrayPath), Path.GetFullPath(listPath), StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Для порівняння виберіть різні файли масиву та списку.");
        var list = new List<CircleModel>(circles);
        circleService.Save(listPath, format, list);
        List<CircleModel> restoredList = circleService.LoadList(listPath, format);
        Console.WriteLine("Відновлений список:");
        PrintCircles(restoredList);
        Console.WriteLine($"Масив: {restoredArray.Length} елементів; список: {restoredList.Count} елементів.");
        Console.WriteLine("Масив має сталу довжину, а до List можна додавати та видаляти елементи.");
        Console.WriteLine("Обидва способи зберігають ті самі поля об’єктів.");
    }
//Обробляє команди меню для роботи зі студентами
    private void WorkWithStudents()
    {
        StorageFormat format = ReadFormat();
        string path = ReadFilePath("students", format);
        while (true)
        {
            Console.WriteLine("\n1 - Показати\n2 - Додати\n3 - Видалити студента\n4 - Порахувати студентів 5 курсу, які служили");
            Console.WriteLine("5 - Створити приклад\n6 - Знайти за квитком\n7 - Видалити файл\n0 - Назад");
            try
            {
                switch (ReadLine())
                {
                    case "1": PrintStudents(entityService.GetStudents(path, format)); break;
                    case "2":
                        entityService.AddStudent(path, format, ToStudentModel(ReadStudentView()));
                        Console.WriteLine("Студента додано.");
                        break;
                    case "3":
                        entityService.RemoveStudent(path, format, ReadText("Номер квитка: "));
                        Console.WriteLine("Студента видалено.");
                        break;
                    case "4":
                        Console.WriteLine("Кількість: " + entityService.CountFifthYearStudentsWhoServed(path, format));
                        break;
                    case "5":
                        if (Confirm("Створити приклад? Наявні дані у файлі буде замінено."))
                            entityService.SaveStudents(path, format, CreateStudents());
                        break;
                    case "6":
                        StudentModel student = entityService.FindStudent(path, format, ReadText("Номер квитка: "));
                        PrintStudents(new StudentModel[] { student });
                        break;
                    case "7":
                        if (Confirm("Видалити файл " + path + "?"))
                            entityService.DeleteStudentsFile(path, format);
                        break;
                    case "0": return;
                    default: Console.WriteLine("Невідомий пункт."); break;
                }
            }
            catch (LabException ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
            }
        }
    }
//Обробляє команди меню для роботи зі спеціалістами
    private void WorkWithSpecialists()
    {
        StorageFormat format = ReadFormat();
        string path = ReadFilePath("specialists", format);
        while (true)
        {
            Console.WriteLine("\n1 - Показати\n2 - Додати\n3 - Видалити запис\n4 - Знайти\n5 - Декламувати вірш\n6 - Видалити файл\n0 - Назад");
            try
            {
                string choice = ReadLine();
                if (choice == "0") return;
                if (choice == "1")
                {
                    foreach (SpecialistModel item in entityService.GetSpecialists(path, format))
                        PrintSpecialist(item);
                }
                else if (choice == "6")
                {
                    if (Confirm("Видалити файл " + path + "?"))
                        entityService.DeleteSpecialistsFile(path, format);
                }
                else if (choice == "2" || choice == "3" || choice == "4" || choice == "5")
                {
                    SpecialistView view = ReadSpecialistView();
                    switch (choice)
                    {
                        case "2":
                            entityService.AddSpecialist(path, format, ToSpecialistModel(view));
                            Console.WriteLine("Запис додано.");
                            break;
                        case "3":
                            entityService.RemoveSpecialist(path, format, view.Type, view.Name);
                            Console.WriteLine("Запис видалено.");
                            break;
                        case "4": PrintSpecialist(entityService.FindSpecialist(path, format, view.Type, view.Name)); break;
                        case "5": Console.WriteLine(entityService.RecitePoetry(path, format, view.Type, view.Name)); break;
                    }
                }
                else Console.WriteLine("Невідомий пункт.");
            }
            catch (LabException ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
            }
        }
    }
//Зчитує та перевіряє професію й ім’я спеціаліста
    private static SpecialistView ReadSpecialistView()
    {
        string type = ReadText("Професія (1 - FootballPlayer, 2 - Lawyer): ");
        if (type == "1") type = "FootballPlayer";
        else if (type == "2") type = "Lawyer";
        else throw new ValidationException("Оберіть 1 або 2.");
        return new SpecialistView { Type = type, Name = ReadText("Ім’я: ") };
    }
//Виводить професію та ім’я спеціаліста
    private static void PrintSpecialist(SpecialistModel model)
    {
        SpecialistView view = ToSpecialistView(model);
        Console.WriteLine(view.Type + ": " + view.Name);
    }
//Зчитує дані студента й перевіряє курс та відповідь про службу
    private static StudentView ReadStudentView()
    {
        var view = new StudentView();
        view.Surname = ReadText("Прізвище: ");
        view.FirstName = ReadText("Ім’я: ");
        while (true)
        {
            string input = ReadText("Курс: ");
            if (int.TryParse(input, out int course) && course > 0)
            {
                view.Course = course;
                break;
            }
            Console.WriteLine("Введіть додатне ціле число.");
        }
        view.StudentTicket = ReadText("Номер студентського квитка: ");
        while (true)
        {
            string answer = ReadText("Служив в армії (так/ні): ").ToLowerInvariant();
            if (answer == "так" || answer == "ні")
            {
                view.ServedInArmy = answer == "так";
                break;
            }
            Console.WriteLine("Введіть так або ні.");
        }
        if (view.ServedInArmy)
            view.MilitaryTicketNumber = ReadText("Номер військового квитка: ");
        return view;
    }
//Зчитує рядок або завершує меню, якщо ввід закінчився
    private static string ReadLine()
    {
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }
//Показує запит і повертає введений текст без крайніх пробілів
    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return ReadLine().Trim();
    }
//Повертає true, якщо користувач підтвердив дію словом «так»
    private static bool Confirm(string message)
    {
        return string.Equals(ReadText(message + " Введіть так для підтвердження: "), "так", StringComparison.OrdinalIgnoreCase);
    }
//Перетворює вибір користувача на формат серіалізації
    private static StorageFormat ReadFormat()
    {
        Console.WriteLine("Формат: 1 - бінарний, 2 - XML, 3 - JSON, 4 - власний");
        switch (ReadLine())
        {
            case "1": return StorageFormat.Binary;
            case "2": return StorageFormat.Xml;
            case "3": return StorageFormat.Json;
            case "4": return StorageFormat.Custom;
            default: throw new ValidationException("Некоректний формат.");
        }
    }
//Визначає розширення та читає шлях до файлу
    private static string ReadFilePath(string name, StorageFormat format)
    {
        string extension;
        switch (format)
        {
            case StorageFormat.Binary: extension = "bin"; name += "-v2"; break;
            case StorageFormat.Xml: extension = "xml"; break;
            case StorageFormat.Json: extension = "json"; break;
            default: extension = "txt"; name += "-v2"; break;
        }
        string defaultPath = name + "." + extension;
        string path = ReadText("Ім’я або шлях файлу [" + defaultPath + "]: ");
        if (path.Length == 0) return defaultPath;
        try
        {
            return Path.GetFullPath(path);
        }
        catch (ArgumentException)
        {
            throw new ValidationException("Некоректний шлях файлу.");
        }
        catch (NotSupportedException)
        {
            throw new ValidationException("Некоректний шлях файлу.");
        }
    }
    //Створює навчальні дані для перевірки підрахунку
//Створює приклад студентських даних для демонстрації підрахунку
    private static List<StudentModel> CreateStudents()
    {
        return new List<StudentModel>
        {
            new StudentModel { Surname = "Бондар", FirstName = "Іван", Course = 5, StudentTicket = "ST-501", ServedInArmy = true, MilitaryTicketNumber = "МВ-101" },
            new StudentModel { Surname = "Мельник", FirstName = "Андрій", Course = 5, StudentTicket = "ST-502", ServedInArmy = true, MilitaryTicketNumber = "МВ-102" },
            new StudentModel { Surname = "Коваленко", FirstName = "Олена", Course = 5, StudentTicket = "ST-503", ServedInArmy = false },
            new StudentModel { Surname = "Романюк", FirstName = "Ірина", Course = 3, StudentTicket = "ST-301", ServedInArmy = false }
        };
    }
//Виводить властивості та обчислення для кожного кола
    private static void PrintCircles(IEnumerable<CircleModel> circles)
    {
        foreach (CircleModel circle in circles)
        {
            CircleView view = ToCircleView(circle);
            Console.WriteLine($"Заливка: {view.FillColor}; контур: {view.OutlineColor}; r = {view.Radius}; S = {circle.Area():F2}; C = {circle.Circumference():F2}");
            }
    }
//Виводить дані кожного студента у консоль
    private static void PrintStudents(IEnumerable<StudentModel> students)
    {
            foreach (StudentModel student in students)
        {
            StudentView view = ToStudentView(student);
            string army = view.ServedInArmy ? "служив" : "не служив";
            Console.WriteLine($"{view.Surname} {view.FirstName}, курс {view.Course}, {view.StudentTicket}, {army}, військовий квиток: {view.MilitaryTicketNumber ?? "—"}");
            }
    }
//Перетворює модель кола BLL на модель для показу
    private static CircleView ToCircleView(CircleModel model)
    {
            return new CircleView
        {
            FillColor = model.FillColor,
            OutlineColor = model.OutlineColor,
            Radius = model.Radius
        };
    }
//Перетворює введені у PL дані на модель BLL
    private static StudentModel ToStudentModel(StudentView view)
    {
        return new StudentModel
        {
            Surname = view.Surname,
            FirstName = view.FirstName,
            Course = view.Course,
            StudentTicket = view.StudentTicket,
            ServedInArmy = view.ServedInArmy,
            MilitaryTicketNumber = view.MilitaryTicketNumber
        };
    }
//Готує модель студента BLL для показу у PL
    private static StudentView ToStudentView(StudentModel model)
    {
        return new StudentView
        {
            Surname = model.Surname,
            FirstName = model.FirstName,
            Course = model.Course,
            StudentTicket = model.StudentTicket,
            ServedInArmy = model.ServedInArmy,
            MilitaryTicketNumber = model.MilitaryTicketNumber
        };
    }
//Перетворює введені у PL дані спеціаліста на модель BLL
    private static SpecialistModel ToSpecialistModel(SpecialistView view)
    {
        return new SpecialistModel
        {
            Type = view.Type,
            Name = view.Name
        };
    }
//Готує модель спеціаліста BLL для показу у PL
    private static SpecialistView ToSpecialistView(SpecialistModel model)
    {
        return new SpecialistView
        {
            Type = model.Type,
            Name = model.Name
        };
    }
}