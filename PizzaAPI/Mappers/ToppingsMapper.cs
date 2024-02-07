using Data.Db.Models.Pizza;
using PizzaAPI.Dtos.Pizza;

namespace PizzaAPI.Mappers
{
    public static class ToppingsMapper
    {
        public static ToppingDto ToDto(this ToppingModel model)
        {
            return new ToppingDto(model.Name, model.Price);
        }

        public static ToppingModel ToModel(this ToppingDto dto)
        {
            return new ToppingModel(dto.Name, dto.Price);
        }
    }
}
