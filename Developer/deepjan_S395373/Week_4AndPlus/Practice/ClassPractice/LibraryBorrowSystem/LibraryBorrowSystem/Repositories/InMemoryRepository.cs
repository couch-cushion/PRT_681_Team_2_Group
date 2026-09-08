
using LibraryBorrowSystem.Interfaces;

namespace LibraryBorrowSystem.Repositories
{
    internal class InMemoryRepository<T> where T : IEntity
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _items.Add(item);
        }

        public T? GetById(int id)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }

        public IReadOnlyList<T> GetAll()
        {
            return _items;
        }
    }
}