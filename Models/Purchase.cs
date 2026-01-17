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
        public int Quantity { get; set; }

        public DateTime PurchaseDate { get; set; }

        public decimal TotalAmount { get; set; }

        public Supplier Supplier { get; set; }
        public Product Product { get; set; }
    }
}
