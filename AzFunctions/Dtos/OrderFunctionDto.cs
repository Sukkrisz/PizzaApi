using ModelLibrary.Shared;

namespace AzFunctions.Dtos
{
    public struct OrderFunctionDto
    {
        public string PhoneNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public Cities City { get; set; }

        public OrderFunctionDto(string phoneNumber, DateTime orderDate, Cities city)
        {
            PhoneNumber = phoneNumber;
            OrderDate = orderDate;
            City = city;
        }
    }
}
