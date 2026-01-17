using System.ComponentModel.DataAnnotations;

namespace IBMS.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required]
        public string SupplierName { get; set; }

        [Required]
        public string ContactNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
