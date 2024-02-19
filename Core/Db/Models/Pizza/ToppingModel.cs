namespace Data.Db.Models.Pizza
{
    public struct ToppingModel : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

        public ToppingModel()
        {
        }

        public ToppingModel(string name, int price)
        {
            Name = name;
            Price = price;
        }
    }
}
