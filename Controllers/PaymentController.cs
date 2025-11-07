using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using BookingHomestay.Data;

namespace BookingHomestay.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Pay(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Đặt phòng không tồn tại.";
                return RedirectToAction("Index", "Booking");
            }

            if (booking.PaymentStatus == "Paid")
            {
                TempData["ErrorMessage"] = "Đặt phòng này đã được thanh toán.";
                return RedirectToAction("Index", "Booking");
            }

            if (booking.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Không thể thanh toán cho đặt phòng đã bị hủy.";
                return RedirectToAction("Index", "Booking");
            }

            booking.PaymentStatus = "Paid";
            _context.Update(booking);

            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = booking.TotalPrice,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Credit Card", // Giả lập phương thức thanh toán
                PaymentStatus = "Paid"
            };
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thanh toán thành công!";
            return RedirectToAction("Index", "Booking");
        }
    }
}