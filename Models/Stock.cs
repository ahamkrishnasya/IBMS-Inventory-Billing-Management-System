using System.ComponentModel.DataAnnotations;

namespace IBMS.Models
{
    public class Stock
    {
        public int StockId { get; set; }

        public int ProductId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int CurrentStock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Reorder level cannot be negative")]
        public int ReorderLevel { get; set; }

        public Product Product { get; set; }
    }
}
