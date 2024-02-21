using ModelLibrary.Shared;

namespace Database.Models.Order
{
    public sealed class OrderModel
    {
        public int Id { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string? Comment { get; set; }

        public AddressModel Address { get; set; }

        public List<OrderedPizzaWithDetails>? Pizzas { get; set; }

        public OrderStatus Status { get; set; }

        public OrderModel()
        {
        }

        /* If it's more readable for the team,
         * to write out all of the property assignments in both constructors,
         * instead of calling the other one with this(..),
         * then it can be agreed on that as well. */
        public OrderModel(
            int id,
            string phoneNumber,
            DateTime orderDate,
            string? comment,
            AddressModel address) : this(phoneNumber, orderDate, comment, address)
        {
            Id = id;
        }

        public OrderModel(
            string phoneNumber,
            DateTime orderDate,
            string? comment,
            AddressModel address)
        {
            PhoneNumber = phoneNumber;
            OrderDate = orderDate;
            Comment = comment;
            Address = address;
        }
    }
}
