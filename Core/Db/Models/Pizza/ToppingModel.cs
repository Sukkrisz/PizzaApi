namespace Data.Db.Models.Pizza
{
    public struct ToppingModel : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ushort Price { get; set; }

        public ToppingModel()
        {
        }

        public ToppingModel(string name, ushort price)
        {
            Name = name;
            Price = price;
        }
    }
}
