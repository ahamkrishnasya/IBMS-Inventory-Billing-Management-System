namespace IBMS.Models
{
    public class Stock
    {
        public int StockId { get; set; }

        public int ProductId { get; set; }

        public int CurrentStock { get; set; }

        public int ReorderLevel { get; set; }

        public Product Product { get; set; }
    }
}
