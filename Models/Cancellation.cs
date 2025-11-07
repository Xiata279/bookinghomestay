using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.Models
{
    public class Cancellation
    {
        [Key]
        public int CancellationId { get; set; }

        [Required]
        public int BookingId { get; set; }

        public string? Reason { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CancellationDate { get; set; } = DateTime.Now;

        public virtual Booking Booking { get; set; } = null!;
    }
}