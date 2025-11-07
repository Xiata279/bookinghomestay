using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.Models
{
    public class HotelFacility
    {
        [Key]
        public int FacilityId { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        [StringLength(100)]
        public string FacilityName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public virtual Hotel Hotel { get; set; } = null!;
    }
}