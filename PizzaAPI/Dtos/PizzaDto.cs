namespace PizzaAPI.Dtos
{
    public struct PizzaDto
    {
        public string Name { get; set; }

        public float BasePrice { get; set; }

        public ToppingDto[] Toppings { get; set; }

        public PizzaDto()
        {
        }

        public PizzaDto(string name, float basePrice, ToppingDto[] toppings)
        {
            Name = name;
            BasePrice = basePrice;
            Toppings = toppings;
        }
    }
}
