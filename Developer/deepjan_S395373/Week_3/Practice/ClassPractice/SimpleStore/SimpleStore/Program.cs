using SimpleStore.Interfaces;
using SimpleStore.Model;
using SimpleStore.Repositories;


try
{
    InMemeoryRepository<Product> productData = new InMemeoryRepository<Product>();

    productData.Add(new Product(1, "Laptop", 1500m));
    productData.Add(new Product(2, "Mouse", 25m));
    productData.Add(new Product(3, "Keyboard", 80m));


    List<Product> getProducts = productData.GetAll();
    foreach (Product product in getProducts)
    {
        Console.WriteLine(product);
    }
    Console.WriteLine();

    productData.RemoveById(2);
    Console.WriteLine();

    productData.PrintAll();
    Console.WriteLine();

    Console.WriteLine("--------------------------------");
    Console.WriteLine();

    InMemeoryRepository<Customer> customerData = new InMemeoryRepository<Customer>();

    customerData.Add(new Customer(1, "Deepjan Thapaliya", "deepjan@email.com"));
    customerData.Add(new Customer(2, "Alex Smith", "alex@email.com"));

    customerData.PrintAll();
    Console.WriteLine();

    customerData.RemoveById(4);
    Console.WriteLine();

    customerData.PrintAll();
    Console.WriteLine();

    customerData.RemoveById(2);
    Console.WriteLine();

    customerData.PrintAll();
    Console.WriteLine();

    customerData.PrintAll();

} catch(ArgumentNullException ex)
{
    Console.WriteLine($"Validation Error: {ex.Message}");
}