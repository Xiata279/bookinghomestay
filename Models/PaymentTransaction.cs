using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm namespace này

namespace BookingHomestay.Models
{
    public class PaymentTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int PaymentId { get; set; }

        [Required]
        [StringLength(100)]
        public string TransactionCode { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionStatus { get; set; } = "Pending";

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public string? GatewayResponse { get; set; }

        public virtual Payment Payment { get; set; } = null!;
    }
}