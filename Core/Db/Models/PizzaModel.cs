using Data.Db.Models;

namespace Core.Data.Models
{
    // Could be a struct, but would need to box & unbox in the multiLoad method
    public class PizzaModel : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public List<ToppingModel> Toppings { get; set; }

        public PizzaModel()
        {
        }

        public PizzaModel(string name, float price, List<ToppingModel> toppings)
        {
            Name = name;
            Price = price;
            Toppings = toppings;
        }
    }
}
