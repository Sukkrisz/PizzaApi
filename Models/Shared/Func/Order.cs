namespace Models.Shared.Func
{
    public struct Order
    {
        //public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public int[] PizzasToBake { get; set; }

        public Order(string phoneNumber, int[] pizzasToBake)
        {
            PhoneNumber = phoneNumber;
            PizzasToBake = pizzasToBake;
        }
    }
}
