using System;

namespace BookingHomestay.Models
{
    public class Payment
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
}