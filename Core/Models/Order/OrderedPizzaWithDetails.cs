using Database.Models.Pizza;
using ModelLibrary.Shared;

namespace Database.Models.Order
{
    public sealed class OrderedPizzaWithDetails : PizzaModel
    {
        public PizzaSize Size { get; set; }
    }
}
