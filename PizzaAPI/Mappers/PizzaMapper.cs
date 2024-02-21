using Database.Models.Pizza;
using PizzaAPI.Dtos.Pizza;

namespace PizzaAPI.Mappers
{
    public static class PizzaMapper
    {
        public static PizzaModel ToDbo(this PizzaDto dto)
        {
            var toppings = dto.Toppings?.Select(t => new ToppingModel(t.Name, t.Price)).ToList();
            return new PizzaModel(dto.Name, dto.BasePrice, toppings);
        }

        public static PizzaDto ToDto(this PizzaModel pizza)
        {
            var toppings = pizza.Toppings?.Select(t => new ToppingDto(t.Name, t.PrepareTime)).ToArray();
            return new PizzaDto(pizza.Name, pizza.Price, toppings);
        }
    }
}
