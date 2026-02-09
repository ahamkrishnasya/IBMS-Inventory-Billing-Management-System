using System;
using System.ComponentModel.DataAnnotations;

namespace IBMS.Models
{
    public class Purchase
    {
        public int PurchaseId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a valid number")]
        public decimal TotalAmount { get; set; }

        public Supplier Supplier { get; set; }
        public Product Product { get; set; }
    }
}
