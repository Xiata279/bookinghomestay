using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BookingHomestay.Data;

namespace BookingHomestay.Controllers
{
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int page = 1)
{
    int pageSize = 10;
    var hotelsQuery = _context.Hotels
        .Include(h => h.Rooms)
        .Include(h => h.Location)
        .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
        hotelsQuery = hotelsQuery.Where(h => h.HotelName.Contains(search) || (h.Location != null && h.Location.City.Contains(search)));
    }

    var totalHotels = await hotelsQuery.CountAsync();
    var hotels = await hotelsQuery
        .OrderBy(h => h.HotelName)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    ViewData["TotalPages"] = (int)Math.Ceiling(totalHotels / (double)pageSize);
    ViewData["CurrentPage"] = page;
    ViewData["Search"] = search;

    return View(hotels);
}

        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Gallery)
                .Include(h => h.Staffs)
                .FirstOrDefaultAsync(h => h.HotelId == id);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }
    }
}