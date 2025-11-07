using BookingHomestay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingHomestay.Data;

namespace BookingHomestay.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Include(b => b.Promotion)
                .Include(b => b.BookingServices)
                .ThenInclude(bs => bs.Service)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Include(b => b.Promotion)
                .Include(b => b.BookingServices)
                .ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        [HttpPost]
public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Kiểm tra chồng chéo thời gian chính xác hơn
    var existingBooking = await _context.Bookings
        .AnyAsync(b => b.RoomId == booking.RoomId &&
                       !(b.CheckOutDate <= booking.CheckInDate || b.CheckInDate >= booking.CheckOutDate));
    if (existingBooking)
    {
        return BadRequest("Phòng này đã được đặt trong khoảng thời gian này.");
    }

    var room = await _context.Rooms.FindAsync(booking.RoomId);
    var days = (booking.CheckOutDate - booking.CheckInDate).Days;
    booking.TotalPrice = days * (room?.Price ?? 0);

    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
}

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, Booking booking)
        {
            if (id != booking.BookingId)
            {
                return BadRequest();
            }

            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            existingBooking.CheckInDate = booking.CheckInDate;
            existingBooking.CheckOutDate = booking.CheckOutDate;
            existingBooking.RoomId = booking.RoomId;
            existingBooking.CustomerId = booking.CustomerId;
            existingBooking.PromotionId = booking.PromotionId;
            existingBooking.Status = booking.Status;
            existingBooking.PaymentStatus = booking.PaymentStatus;

            var room = await _context.Rooms.FindAsync(booking.RoomId);
            var days = (booking.CheckOutDate - booking.CheckInDate).Days;
            existingBooking.TotalPrice = days * (room?.Price ?? 0);

            _context.Entry(existingBooking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}