using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingHomestay.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal RoomPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ServicePrice { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Tax { get; set; } = 0.00m;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        public virtual Booking Booking { get; set; } = null!;
    }
}