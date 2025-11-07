using System.Collections.Generic;
using BookingHomestay.Models;

namespace BookingHomestay.ViewModels
{
    public class HotelViewModel
    {
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int? LocationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Gallery> Gallery { get; set; } = new List<Gallery>();
        public List<Staff> Staffs { get; set; } = new List<Staff>();
    }
}