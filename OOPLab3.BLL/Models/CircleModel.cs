namespace OOPLab3.BLL.Models;
//Модель кола на рівні бізнес-логіки
public sealed class CircleModel
{
    public string FillColor { get; set; } = string.Empty;
    public string OutlineColor { get; set; } = string.Empty;
    public double Radius { get; set; }
    //Обчислює площу кола за радіусом
    public double Area() => Math.PI * Radius * Radius;
    //Обчислює довжину окружності
    public double Circumference() => 2 * Math.PI * Radius;
    //Повертає рядок з усіма даними про коло
    public string Describe() => $"Коло: заливка {FillColor}, контур {OutlineColor}, радіус {Radius:0.##}, площа {Area():0.##}, довжина {Circumference():0.##}";
}