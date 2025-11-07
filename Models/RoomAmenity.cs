using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.Models
{
    public class RoomAmenity
    {
        [Key]
        public int AmenityId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [StringLength(100)]
        public string AmenityName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public virtual Room Room { get; set; } = null!;
    }
}