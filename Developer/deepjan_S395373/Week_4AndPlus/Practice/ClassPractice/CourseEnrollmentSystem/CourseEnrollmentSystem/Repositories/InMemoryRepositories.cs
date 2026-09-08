using CourseEnrollmentSystem.Interfaces;

namespace CourseEnrollmentSystem.Repositories
{
    internal class InMemoryRepository<T> where T : IEntity
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

        public IReadOnlyList<T> GetAll()
        {
            return _items;
        }

        public bool RemoveById(int id)
        {
            T? itemToRemove = GetById(id);

            if (itemToRemove == null)
            {
                return false;
            }

            _items.Remove(itemToRemove);
            return true;
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