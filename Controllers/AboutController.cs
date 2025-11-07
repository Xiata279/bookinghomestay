using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookingHomestay.Controllers
{
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels
                .Include(h => h.Staffs)
                .ToListAsync();

            return View(hotels);
        }
    }
}