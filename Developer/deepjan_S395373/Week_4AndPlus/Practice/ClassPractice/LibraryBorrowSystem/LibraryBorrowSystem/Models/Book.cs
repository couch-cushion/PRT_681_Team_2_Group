
using LibraryBorrowSystem.Interfaces;

namespace LibraryBorrowSystem.Models
{
    internal class Book : IEntity
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public bool IsAvailable { get; private set; }

        public Book(int id, string title, string author, bool isAvailable)
        {
            if (id < 1)
            {
                throw new ArgumentException("Book id must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Book title cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Book author cannot be empty.");
            }

            Id = id;
            Title = title;
            Author = author;
            IsAvailable = isAvailable;
        }

        public void MarkAsBorrowed()
        {
            IsAvailable = false;
        }

        public override string ToString()
        {
            string status = IsAvailable ? "Available" : "Borrowed";
            return $"{Id} - {Title} by {Author} - {status}";
        }
    }
}
