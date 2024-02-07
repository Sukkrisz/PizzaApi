using Data.Db.Models.Pizza;
namespace Core.Data.BL.Extensions
{
    public static class PizzaExtensions
    {
        public static float GetPrice(this PizzaModel pizzaDto)
        {
            return 1F;
        }
    }
}
