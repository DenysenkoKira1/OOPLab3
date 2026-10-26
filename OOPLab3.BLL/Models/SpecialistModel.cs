namespace OOPLab3.BLL.Models;
//Для двох професій потрібні однакові дані, тому достатньо одного класу з полем Type
//Модель спеціаліста бізнес-логіки для двох професій
public class SpecialistModel
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
//Повертає текст декламації, доступний для спеціаліста
    public string RecitePoetry()
    {
        return Name + " декламує:\n" +
            "Рядок за рядком я програму складаю,\n" +
            "Перевіряю помилки — і далі навчаюсь.";
    }
}