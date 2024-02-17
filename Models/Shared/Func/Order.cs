namespace Models.Shared.Func
{
    public struct Order
    {
        public string PhoneNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public Cities City { get; set; }

        public Order(string phoneNumber, DateTime orderDate, Cities city)
        {
            PhoneNumber = phoneNumber;
            OrderDate = orderDate;
            City = city;
        }
    }
}
