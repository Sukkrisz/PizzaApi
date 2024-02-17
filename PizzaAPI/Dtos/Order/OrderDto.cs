namespace PizzaAPI.Dtos.Order
{
    public class OrderDto
    {
        public string PhoneNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string? Comment { get; set; }

        public AddressDto Address { get; set; }

        public OrderedPizzaDto[] Pizzas { get; set; }
    }
}
