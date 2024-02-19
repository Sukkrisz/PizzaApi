namespace PizzaAPI.Dtos.Pizza
{
    public struct ToppingDto
    {
        public string Name { get; set; }
        public int Price { get; set; }

        public ToppingDto()
        {
        }

        public ToppingDto(string name, int price)
        {
            Name = name;
            Price = price;
        }
    }
}
