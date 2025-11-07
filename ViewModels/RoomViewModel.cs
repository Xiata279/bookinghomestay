using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookingHomestay.Models;

namespace BookingHomestay.ViewModels
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Tên phòng là bắt buộc")]
        public string RoomName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại phòng là bắt buộc")]
        public string RoomType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số phòng là bắt buộc")]
        public string RoomNumber { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public string AvailabilityStatus { get; set; } = "Available";
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? LocationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual Hotel? Hotel { get; set; }
        public virtual Location? Location { get; set; }
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
        public virtual ICollection<Gallery> Gallery { get; set; } = new List<Gallery>();
        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    }
}