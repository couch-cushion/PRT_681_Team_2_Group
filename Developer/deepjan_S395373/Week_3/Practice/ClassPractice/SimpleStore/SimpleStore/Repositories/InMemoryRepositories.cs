using SimpleStore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Repositories
{
    internal class InMemeoryRepository<T> where T : IEntity
    {
        private readonly List<T> _items = new List<T>();


        public void Add(T item)
        {
            _items.Add(item);
        }
        public T? GetById(int id)
        {
            foreach (T item in _items)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return default;
        }
        public List<T> GetAll()
        {
            return _items;
        }
        public void RemoveById(int id)
        {
            T? itemToRemove = GetById(id);
            Console.WriteLine($"To Remove: {itemToRemove}");
            if (itemToRemove != null)
            {
                
                _items.Remove(itemToRemove);
                Console.WriteLine($"Removed successfully");
            }
            else
            {
                Console.WriteLine($"Remove data unsuccessfull");
            }
        }

        public void PrintAll()
        {
            foreach (T item in _items)
            {
                Console.WriteLine(item);
            }
        }
    }
}
