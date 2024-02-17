using Data.Db.Models.Order;
using PizzaAPI.Dtos.Order;

namespace PizzaAPI.Mappers
{
    public static class OrderedPizzaMapper
    {
        public static OrderedPizzaModel ToModel(this OrderedPizzaDto dto)
        {
            return new OrderedPizzaModel() { Id = dto.Id, Size = dto.Size };
        }
    }
}
