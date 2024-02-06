namespace PizzaAPI.Dtos
{
    public struct ToppingDto
    {
        public string Name { get; set; }
        public float Price { get; set; }

        public ToppingDto()
        {
        }

        public ToppingDto(string name, float price)
        {
            Name = name;
            Price = price;
        }
    }
}
