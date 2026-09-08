using System;
using System.Collections.Generic;
using System.Text;
using SimpleStore.Interfaces;

namespace SimpleStore.Model
{
    internal class Product:IEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public Product(int id, string name, decimal price)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Product's id Must be" +
                    "greater than 0.");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product's name Cannot" +
                    "be Empty.");
            }
            if (price < 0)
            {
                throw new ArgumentException("Product price cannot be negative.");
            }

            Id = id;
            Name = name;
            Price = price;
        }

        public override string ToString()
        {
            return $"Product: {Id} - {Name} - ${Price}";
        }

    }
}
