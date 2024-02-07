namespace PizzaAPI.Dtos.Order
{
    public struct OrderCommandDto
    {
        public string PhoneNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string? Comment { get; set; }

        public AddressDto Address { get; set; }

        public int[] PizzaIds { get; set; }

        public OrderCommandDto(string phoneNumber, DateTime orderDate, string? comment, AddressDto address, int[] pizzaIds)
        {
            PhoneNumber = phoneNumber;
            OrderDate = orderDate;
            Comment = comment;
            Address = address;
            PizzaIds = pizzaIds;
        }
    }
}
