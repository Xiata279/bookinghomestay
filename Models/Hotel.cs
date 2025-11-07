using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.Models
{
    public class Hotel
    {
        [Key]
        public int HotelId { get; set; }

        [Required]
        [StringLength(255)]
        public string? HotelName { get; set; } // Cho phép null

        [StringLength(1000)]
        public string? Description { get; set; } // Cho phép null

        [StringLength(255)]
        public string? ImageUrl { get; set; } // Cho phép null

        public int? LocationId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public virtual Location? Location { get; set; }
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
        public virtual ICollection<Staff> Staffs { get; set; } = new List<Staff>();
        public virtual ICollection<Gallery> Gallery { get; set; } = new List<Gallery>();
        public virtual ICollection<HotelFacility> HotelFacilities { get; set; } = new List<HotelFacility>();
    }
}