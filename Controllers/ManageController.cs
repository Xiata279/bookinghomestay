using BookingHomestay.Data;
using BookingHomestay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookingHomestay.Controllers
{
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageSize = 10;
            var bookingsQuery = _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Include(b => b.Customer)
                .Include(b => b.Promotion)
                .Include(b => b.BookingServices)
                .ThenInclude(bs => bs.Service)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                bookingsQuery = bookingsQuery.Where(b =>
                    b.Customer.FullName.Contains(search) ||
                    b.Room.RoomName.Contains(search) ||
                    b.Room.Hotel.HotelName.Contains(search));
            }

            var totalBookings = await bookingsQuery.CountAsync();
            var bookings = await bookingsQuery
                .OrderByDescending(b => b.CheckInDate) // Dòng 43: Thay BookingDate bằng CheckInDate
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling(totalBookings / (double)pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["Search"] = search;

            return View(bookings);
        }
    }
}