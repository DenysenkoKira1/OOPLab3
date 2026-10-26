using OOPLab3.BLL.Models;
using OOPLab3.DAL.Entities;
using OOPLab3.DAL.Storage;
namespace OOPLab3.BLL.Services;
//Перетворює моделі кіл і передає їх для збереження або читання на рівень DAL
public class CircleService
{
    //IList підтримує і масив, і список. Тип контейнера зберігаємо під час перетворення
//Перевіряє кола, зберігаючи тип колекції, і передає сутності в DAL
    public void Save(string path, StorageFormat format, IList<CircleModel> circles)
    {
        bool asArray = circles is CircleModel[];
        IList<CircleEntity> entities;
        if (asArray) entities = new CircleEntity[circles.Count];
        else entities = new List<CircleEntity>();
        for (int i = 0; i < circles.Count; i++)
        {
            CircleModel model = circles[i];
            if (model.Radius <= 0 || !double.IsFinite(model.Radius))
                throw new ValidationException("Радіус має бути додатним скінченним числом.");
            var entity = new CircleEntity
            {
                FillColor = model.FillColor,
                OutlineColor = model.OutlineColor,
                Radius = model.Radius
            };
            if (asArray) entities[i] = entity;
            else entities.Add(entity);
        }
        try
        {
            var context = new EntityContext<CircleEntity>(format);
            context.Save(path, entities);
        }
        catch (DataAccessException ex)
        {
            throw new StorageOperationException(ex.Message, ex);
        }
    }
//Завантажує кола у вигляді масиву
    public CircleModel[] LoadArray(string path, StorageFormat format)
    {
        return (CircleModel[])Load(path, format, true);
    }
//Завантажує кола у вигляді списку
    public List<CircleModel> LoadList(string path, StorageFormat format)
    {
        return (List<CircleModel>)Load(path, format, false);
    }
//Читає сутності, перевіряє їх і перетворює на моделі BLL
    private IList<CircleModel> Load(string path, StorageFormat format, bool asArray)
    {
        try
        {
            var context = new EntityContext<CircleEntity>(format);
            IList<CircleEntity> entities = context.Load(path, asArray);
            IList<CircleModel> result;
            if (asArray) result = new CircleModel[entities.Count];
            else result = new List<CircleModel>();
            for (int i = 0; i < entities.Count; i++)
            {
                CircleEntity entity = entities[i];
                if (entity == null || entity.Radius <= 0 || !double.IsFinite(entity.Radius))
                    throw new ValidationException("Файл містить некоректне коло.");
                var model = new CircleModel
                {
                    FillColor = entity.FillColor,
                    OutlineColor = entity.OutlineColor,
                    Radius = entity.Radius
                };
                if (asArray) result[i] = model;
                else result.Add(model);
            }
            return result;
        }
        catch (DataAccessException ex)
        {
            throw new StorageOperationException(ex.Message, ex);
        }
    }
//Створює шість навчальних кіл для демонстрації серіалізації
    public CircleModel[] CreateSixExamples()
    {
        return new CircleModel[]
        {
            new CircleModel { FillColor = "червоний", OutlineColor = "чорний", Radius = 1 },
            new CircleModel { FillColor = "зелений", OutlineColor = "білий", Radius = 2 },
            new CircleModel { FillColor = "синій", OutlineColor = "жовтий", Radius = 3 },
            new CircleModel { FillColor = "жовтий", OutlineColor = "синій", Radius = 4 },
            new CircleModel { FillColor = "фіолетовий", OutlineColor = "сірий", Radius = 5 },
            new CircleModel { FillColor = "помаранчевий", OutlineColor = "коричневий", Radius = 6 }
        };
    }
}