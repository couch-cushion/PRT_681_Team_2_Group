using LibraryBorrowSystem.Common;
using LibraryBorrowSystem.Models;
using LibraryBorrowSystem.Repositories;
using LibraryBorrowSystem.Services;

try
{
    InMemoryRepository<Book> bookRepository = new InMemoryRepository<Book>();

    bookRepository.Add(new Book(1, "Clean Code", "Robert C. Martin", true));
    bookRepository.Add(new Book(2, "The Pragmatic Programmer", "Andrew Hunt", true));
    bookRepository.Add(new Book(3, "C# in Depth", "Jon Skeet", false));

    LibraryService libraryService = new LibraryService(bookRepository);

    // TODO:
    // Call FindBookById(1)
    // Print result message
    // If success, print book data

    Result<Book> result = libraryService.FindBookById(1);
    Console.WriteLine(result.Message);
    if (result.IsSuccess)
    {
        Console.WriteLine(result.Data);
    }

    Console.WriteLine();

    // TODO:
    // Call BorrowBook(2, "Deepjan")
    // Print result message
    // If success, print receipt data

    Result<BorrowReceipt> result1 = libraryService.BorrowBook(2, "Deepjan");
    Console.WriteLine(result1.Message);

    if (result1.IsSuccess)
    {
        Console.WriteLine(result1.Data); ;
    }
    
    Console.WriteLine();

    // TODO:
    // Call BorrowBook(3, "Deepjan")
    // This should fail because book is already borrowed
    Result<BorrowReceipt> result2 = libraryService.BorrowBook(3, "Deepjan");
    Console.WriteLine(result2.Message);

    if (result2.IsSuccess)
    {
        Console.WriteLine(result2.Data); ;
    }


    Console.WriteLine();

    // TODO:
    // Call BorrowBook(99, "Deepjan")
    // This should fail because book does not exist
    Result<BorrowReceipt> result3 = libraryService.BorrowBook(99, "Deepjan");
    Console.WriteLine(result3.Message);

    if (result3.IsSuccess)
    {
        Console.WriteLine(result3.Data); ;
    }

}
catch (ArgumentException ex)
{
    Console.WriteLine($"Validation Error: {ex.Message}");
}
