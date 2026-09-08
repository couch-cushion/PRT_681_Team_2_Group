using CleanCartSystem.Common;
using CleanCartSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanCartSystem.Output
{
    internal class CartConsolePrinter
    {
        public void  PrintCartSummary(Result<CartSummary> result)
        {
            Console.WriteLine(result.Message);

            if (!result.IsSuccess || result.Data == null)
            {
                return;
            }

            CartSummary cartSummary = result.Data;

            Console.WriteLine();
            Console.WriteLine("Cart Summary:");
            Console.WriteLine(cartSummary.Customer);
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine();

            foreach (CartItem item in cartSummary.Items)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            Console.WriteLine($"Subtotal: ${cartSummary.Subtotal}");
            Console.WriteLine($"Discount: ${cartSummary.Discount}");
            Console.WriteLine($"Final Total: ${cartSummary.FinalTotal}");
        }
        
        public void PrintResult<T>(Result<T> result)
        {
            Console.WriteLine($"\n{result.Message}");

            if (!result.IsSuccess || result.Data == null)
            {
                return;
            }
            Console.WriteLine(result.Data);
        }

        public void PrintOrderReceipt(Result<OrderReceipt> orderReceipt)
        {
            Console.WriteLine(orderReceipt.Message);

            if (!orderReceipt.IsSuccess || orderReceipt.Data == null)
            {
                return;
            }

            OrderReceipt orderReceiptData = orderReceipt.Data;

            Console.WriteLine();
            Console.WriteLine("Order Receipt:");
            Console.WriteLine(orderReceiptData.Customer);
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine();

            foreach (CartItem item in orderReceiptData.Items)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            Console.WriteLine($"Subtotal: ${orderReceiptData.Subtotal}");
            Console.WriteLine($"Discount: ${orderReceiptData.Discount}");
            Console.WriteLine($"Final Total: ${orderReceiptData.FinalTotal}");
            Console.WriteLine($"Checked out at: ${orderReceiptData.CheckedoutAt}");
        }
    }
}
