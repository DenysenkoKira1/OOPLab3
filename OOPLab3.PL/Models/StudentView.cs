namespace OOPLab3.PL.Models;
//Модель студента для введення та виведення у консолі
//Модель даних студента, яку використовує консольний інтерфейс
public class StudentView
{
    public string Surname { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public int Course { get; set; }
    public string StudentTicket { get; set; } = string.Empty;
    public bool ServedInArmy { get; set; }
    public string? MilitaryTicketNumber { get; set; }
}