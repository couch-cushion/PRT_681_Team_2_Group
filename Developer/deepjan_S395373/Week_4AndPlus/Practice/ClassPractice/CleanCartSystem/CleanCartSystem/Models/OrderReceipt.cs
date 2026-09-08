using System;
using System.Collections.Generic;
using System.Text;

namespace CleanCartSystem.Models
{
    internal class OrderReceipt
    {
        public Customer Customer { get; private set; }
        public IReadOnlyList<CartItem> Items { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal Discount { get; private set; }
        public decimal FinalTotal { get; private set; }
        public DateTime CheckedoutAt { get; private set; }

        public OrderReceipt(
            Customer customer,
            IReadOnlyList<CartItem> items,
            decimal subtotal,
            decimal discount,
            decimal finalTotal,
            DateTime checkedOutAt)
        {

            Customer = customer ?? throw new ArgumentNullException("Customer is not found");
            Items = items ?? throw new ArgumentNullException("Cart is Empty");
            if (subtotal < 0)
            {
                throw new ArgumentException("Subtotal cannot be negative.");
            }

            if (discount < 0)
            {
                throw new ArgumentException("Discount cannot be negative.");
            }

            if (finalTotal < 0)
            {
                throw new ArgumentException("Final total cannot be negative.");
            }
            Subtotal = subtotal;
            Discount = discount;
            FinalTotal = finalTotal;
            CheckedoutAt = checkedOutAt;
        }

            public override string ToString()
        {
            return $"{Customer.CustomerName} checked out {Items.Count} item(s). Final total: ${FinalTotal}";
        }
    }
}
