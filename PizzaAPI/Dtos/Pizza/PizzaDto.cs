namespace PizzaAPI.Dtos.Pizza
{
    public struct PizzaDto
    {
        public string Name { get; set; }

        public ushort BasePrice { get; set; }

        public ToppingDto[] Toppings { get; set; }

        public PizzaDto()
        {
        }

        public PizzaDto(string name, ushort basePrice, ToppingDto[] toppings)
        {
            Name = name;
            BasePrice = basePrice;
            Toppings = toppings;
        }
    }
}
