using System;
using System.Collections.Generic;

namespace BookingHomestay.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; } // Cho phép null
        public string? PaymentStatus { get; set; } // Cho phép null
        public int? PromotionId { get; set; }
        public Promotion Promotion { get; set; }
        public DateTime BookingDate { get; set; }
        public List<BookingService> BookingServices { get; set; } = new List<BookingService>();
        public List<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}