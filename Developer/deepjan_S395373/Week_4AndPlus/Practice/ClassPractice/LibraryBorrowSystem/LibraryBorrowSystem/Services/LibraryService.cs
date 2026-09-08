using LibraryBorrowSystem.Models;
using LibraryBorrowSystem.Common;
using LibraryBorrowSystem.Repositories;


namespace LibraryBorrowSystem.Services
{
    internal class LibraryService
    {
        private readonly InMemoryRepository<Book> _bookRepository;

        public LibraryService(InMemoryRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        }

        public Result<Book> FindBookById(int bookId)
        {
            if (bookId < 1)
            {
                return  Result<Book>.Failure("Book id must be greater than zero");
            }

            Book? book = _bookRepository.GetById(bookId);

            if (book == null)
            {
                return Result<Book>.Failure("Book not found.");
            }

            return Result<Book>.Success("Book found.", book);
        }

        public Result<BorrowReceipt> BorrowBook(int bookId, string borrowerName)
        {
            if (bookId < 1)
            {
                return Result<BorrowReceipt>.Failure("Book id must be greater than zero");
            }

            if (string.IsNullOrWhiteSpace(borrowerName))
            {
                return Result<BorrowReceipt>.Failure("Borrower name cannot be empty");
            }

            Book? book = _bookRepository.GetById(bookId);

            if (book == null)
            {
                return Result<BorrowReceipt>.Failure("Book not found");

            }

            if (!book.IsAvailable)
            {
                return Result<BorrowReceipt>.Failure("Book not Available");
            }

            book.MarkAsBorrowed();

            BorrowReceipt receipt = new BorrowReceipt(
                book,
                borrowerName,
                DateTime.Now
            );

            return Result<BorrowReceipt>.Success("Book borrowed Successfully",receipt);
            
        }
    }
}