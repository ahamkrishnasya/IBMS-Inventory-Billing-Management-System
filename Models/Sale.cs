using System;
using System.ComponentModel.DataAnnotations;

namespace IBMS.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }

        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }
}
