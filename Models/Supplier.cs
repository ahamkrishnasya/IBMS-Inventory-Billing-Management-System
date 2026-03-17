using System.ComponentModel.DataAnnotations;

namespace IBMS.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(100)]
        public string SupplierName { get; set; }

        [Required]
        [Phone]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be exactly 10 digits.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must contain exactly 10 digits.")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250)]
        public string Address { get; set; }
    }
}
