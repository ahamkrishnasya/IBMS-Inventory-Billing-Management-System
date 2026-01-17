using System;

namespace IBMS.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        public int SaleId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string PaymentMode { get; set; }

        public decimal InvoiceTotal { get; set; }

        public Sale Sale { get; set; }
    }
}
