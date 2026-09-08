using CleanCartSystem.Calculators;
using CleanCartSystem.Common;
using CleanCartSystem.Interfaces;
using CleanCartSystem.Models;
using CleanCartSystem.Output;
using CleanCartSystem.Repositories;
using CleanCartSystem.Services;
using System.Reflection.Metadata;

try
{
    InMemoryRepository<Customer> customerRepository = new InMemoryRepository<Customer>();
    InMemoryRepository<Product> productRepository = new InMemoryRepository<Product>();

    productRepository.Add(new Product(1, "Laptop", 1500m));
    productRepository.Add(new Product(2, "Mouse", 25m));
    productRepository.Add(new Product(3, "Keyboard", 80m));
    productRepository.Add(new Product(4, "Monitor", 300m));

    customerRepository.Add(new Customer(1, "Deepjan Thapaliya", true));
    customerRepository.Add(new Customer(2, "Alex Smith", false));

    CartService cartService = new CartService(customerRepository, productRepository, new SimpleDiscountCalculator());

    CartConsolePrinter printer = new CartConsolePrinter();

    Result<CartItem> result1 = cartService.AddItemToCart(1,1, 1);
    printer.PrintResult(result1);

    Result<CartItem> result2 = cartService.AddItemToCart(1,1, 2);
    printer.PrintResult(result2);

    Result<CartItem> result3 = cartService.AddItemToCart(2, 3, 1);
    printer.PrintResult(result3);

    Result<CartItem> result4 = cartService.AddItemToCart(2,99, 1);
    printer.PrintResult(result4);

    Result<CartItem> result5 = cartService.AddItemToCart(1, 2, 3);
    printer.PrintResult(result5);

    Result<CartItem> result6 = cartService.AddItemToCart(2, 2, 2);
    printer.PrintResult(result6);

    Console.WriteLine();

    Result<CartSummary> customer1Cart = cartService.GetCartSummary(1);
    printer.PrintCartSummary(customer1Cart);

    Console.WriteLine();


    Result<CartItem> decreaseResult = cartService.DecreaseItemQuantity(1, 2, 2);
    printer.PrintResult(decreaseResult);
    Console.WriteLine();

    Result<CartSummary> updatedCustomer1Cart = cartService.GetCartSummary(1);
    printer.PrintCartSummary(updatedCustomer1Cart);

    Console.WriteLine();

    Result<CartItem> removeResult = cartService.RemoveItemFromCart(1, 2);
    printer.PrintResult(removeResult);
    Console.WriteLine();

    Result<CartSummary> finalCustomer1Cart = cartService.GetCartSummary(1);
    printer.PrintCartSummary(finalCustomer1Cart);

    Console.WriteLine();
    Result<OrderReceipt> orderReceipt = cartService.Checkout(1);
    
    printer.PrintOrderReceipt(orderReceipt);
    Result<CartSummary> customer1CartAfterCheckout = cartService.GetCartSummary(1);
    printer.PrintCartSummary(customer1CartAfterCheckout);


}
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message);
}
