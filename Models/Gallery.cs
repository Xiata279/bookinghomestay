using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.Models
{
    public class Gallery
{
    [Key]
    public int GalleryId { get; set; }

    public int? RoomId { get; set; }

    public int? HotelId { get; set; }

    [Required]
    [StringLength(255)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public virtual Room? Room { get; set; }
    public virtual Hotel? Hotel { get; set; }
}
}