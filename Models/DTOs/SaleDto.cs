using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IBMS.Models.DTOs
{
    public class SaleDto
    {
        public int SaleId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }

        // UI SUPPORT (NOT DB)
        public SelectList CustomerList { get; set; }
        public SelectList ProductList { get; set; }

        // 🔥 FOR AUTO CALCULATION
        public Dictionary<int, decimal> ProductPrices { get; set; }
        public Dictionary<int, decimal> ProductTaxRates { get; set; }
    }
}
