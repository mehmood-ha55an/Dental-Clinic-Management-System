using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public string InvoiceNumber { get; set; } = null!;

        [Required]
        public string PatientName { get; set; } = null!;

        public string? Phone { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();
    }


    public class InvoiceItem
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }

        public decimal Amount => Quantity * UnitPrice;

        public Invoice Invoice { get; set; } = null!;
    }


}
