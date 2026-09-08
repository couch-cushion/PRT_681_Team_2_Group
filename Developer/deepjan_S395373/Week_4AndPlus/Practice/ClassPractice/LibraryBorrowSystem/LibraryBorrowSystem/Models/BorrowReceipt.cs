namespace LibraryBorrowSystem.Models
{
    internal class BorrowReceipt
    {
        public Book Book { get; private set; }
        public string BorrowerName { get; private set; }
        public DateTime BorrowedAt { get; private set; }

        public BorrowReceipt(Book book, string borrowerName, DateTime borrowedAt)
        {
            Book = book ?? throw new ArgumentNullException(nameof(book));

            if (string.IsNullOrWhiteSpace(borrowerName))
            {
                throw new ArgumentException("Borrower name cannot be empty.");
            }

            BorrowerName = borrowerName;
            BorrowedAt = borrowedAt;
        }

        public override string ToString()
        {
            return $"{BorrowerName} borrowed '{Book.Title}' at {BorrowedAt}";
        }
    }
}