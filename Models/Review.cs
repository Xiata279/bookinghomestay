using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.Models
{
    public class Review
{
    [Key]
    public int ReviewId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending";

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime ReviewDate { get; set; } = DateTime.Now;

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.DateTime)]
    public DateTime? ModifiedAt { get; set; }

    public int? BookingId { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual Room Room { get; set; } = null!;
    public virtual Booking? Booking { get; set; }
}
}