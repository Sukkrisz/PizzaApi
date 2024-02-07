using PizzaAPI.Dtos.Pizza;

namespace PizzaAPI.Dtos.Order
{
    public struct OrderQueryResultDto
    {
        public string PhoneNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string? Comment { get; set; }

        public AddressDto Address { get; set; }

        public int MyProperty { get; set; }

        public PizzaDto[] Pizzas { get; set; }

        public int Price
        {
            get { return Pizzas.Sum(p => p.BasePrice + p.Toppings.Sum(t => t.Price)); } 
        }

        public OrderQueryResultDto(string phoneNumber, DateTime orderDate, string? comment, AddressDto address, PizzaDto[] pizzas)
        {
            PhoneNumber = phoneNumber;
            OrderDate = orderDate;
            Comment = comment;
            Address = address;
            Pizzas = pizzas;
        }
    }
}
