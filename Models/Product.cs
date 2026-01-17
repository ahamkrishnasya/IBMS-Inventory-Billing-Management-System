using System.ComponentModel.DataAnnotations;

namespace IBMS.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal TaxRate { get; set; }
    }
}
