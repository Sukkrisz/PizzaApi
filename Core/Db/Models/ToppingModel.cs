using Data.Db.Models;

namespace Core.Data.Models
{
    public struct ToppingModel : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }

        public ToppingModel()
        {
        }

        public ToppingModel(string name, float price)
        {
            Name = name;
            Price = price;
        }
    }
}
