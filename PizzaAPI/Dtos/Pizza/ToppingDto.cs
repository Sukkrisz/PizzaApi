namespace PizzaAPI.Dtos.Pizza
{
    public struct ToppingDto
    {
        public string Name { get; set; }
        public ushort Price { get; set; }

        public ToppingDto()
        {
        }

        public ToppingDto(string name, ushort price)
        {
            Name = name;
            Price = price;
        }
    }
}
