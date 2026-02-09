using System;
using System.ComponentModel.DataAnnotations;

namespace IBMS.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        [Required]
        public int SaleId { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required(ErrorMessage = "Payment mode is required")]
        [StringLength(50)]
        public string PaymentMode { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Invoice total must be greater than 0")]
        public decimal InvoiceTotal { get; set; }

        public Sale Sale { get; set; }
    }
}
