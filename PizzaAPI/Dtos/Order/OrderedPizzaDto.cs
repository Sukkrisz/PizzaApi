using ModelLibrary.Shared;

namespace PizzaAPI.Dtos.Order
{
    public struct OrderedPizzaDto
    {
        public int Id { get; set; }
        public PizzaSize Size { get; set; }
    }
}
