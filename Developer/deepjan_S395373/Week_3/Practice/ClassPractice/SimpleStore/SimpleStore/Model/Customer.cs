using System;
using System.Collections.Generic;
using System.Text;
using SimpleStore.Interfaces;

namespace SimpleStore.Model
{
    internal class Customer:IEntity
    {
        public  int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }

        public Customer(int id, string name, string email)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Customer's id Must be" +
                    "greater than 0.");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Customer's name Cannot" +
                    "be Empty.");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Customer's email Cannot" +
                    "be Empty.");
            }

            Id = id;
            Name = name;
            Email = email;
        }

        public override string ToString()
        {
            return $"Customer: {Id} - {Name} - {Email}";
        }
    }
}
