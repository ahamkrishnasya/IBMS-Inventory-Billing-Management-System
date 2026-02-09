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
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal NetAmount { get; set; }

        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }
}
