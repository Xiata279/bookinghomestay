using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingHomestay.Models
{
    public class BookingService
{
    [Key]
    public int BookingServiceId { get; set; }

    [Required]
    public int BookingId { get; set; }

    [Required]
    public int ServiceId { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Booking Booking { get; set; } = null!;
    public virtual Service Service { get; set; } = null!;
}
}