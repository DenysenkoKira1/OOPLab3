namespace OOPLab3.BLL.Models;
//Модель студента на рівні бізнес-логіки
//Модель студента бізнес-логіки
public sealed class StudentModel
{
    public string Surname { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public int Course { get; set; }
    public string StudentTicket { get; set; } = string.Empty;
    public bool ServedInArmy { get; set; }
    public string? MilitaryTicketNumber { get; set; }
}