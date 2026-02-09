using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IBMS.Models.DTOs
{
    public class PurchaseDto
    {
        public int PurchaseId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        public decimal TotalAmount { get; set; }

        // 🔑 UI SUPPORT (NOT DB)
        public IEnumerable<SelectListItem> SupplierList { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }

        // 🔑 FOR JS AUTO CALCULATION
        public Dictionary<int, decimal> ProductPrices { get; set; }
    }
}
