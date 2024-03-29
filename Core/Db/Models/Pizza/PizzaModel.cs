﻿using Data.Db.Models;

namespace Data.Db.Models.Pizza
{
    /* Could be a struct, but would need to box & unbox in the multiLoad method
     * and the method is hard to read as is. */
    public class PizzaModel : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ushort Price { get; set; }

        public List<ToppingModel> Toppings { get; set; }

        public PizzaModel()
        {
        }

        public PizzaModel(string name, ushort price, List<ToppingModel> toppings)
        {
            Name = name;
            Price = price;
            Toppings = toppings;
        }
    }
}
