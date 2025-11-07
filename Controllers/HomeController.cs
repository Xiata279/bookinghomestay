using BookingHomestay.Data;
using BookingHomestay.Models;
using BookingHomestay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookingHomestay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                AvailableRooms = await _context.Rooms
                    .Include(r => r.Hotel)
                    .Include(r => r.Location)
                    .Where(r => r.IsActive)
                    .Take(5)
                    .Select(r => new RoomViewModel
                    {
                        RoomId = r.RoomId,
                        HotelId = r.HotelId,
                        RoomName = r.RoomName,
                        RoomType = r.RoomType,
                        RoomNumber = r.RoomNumber,
                        Price = r.Price,
                        AvailabilityStatus = r.AvailabilityStatus,
                        Description = r.Description,
                        ImageUrl = r.ImageUrl,
                        LocationId = r.LocationId,
                        Location = r.Location,
                        CreatedAt = r.CreatedAt,
                        Hotel = r.Hotel,
                        IsActive = r.IsActive
                    })
                    .ToListAsync(),
                Promotions = await _context.Promotions
    .Where(p => p.IsActive)
    .Take(3)
    .Select(p => new PromotionViewModel
    {
        PromotionId = p.PromotionId,
        Name = p.Name,
        PromotionCode = p.PromotionCode,
        Description = p.Description,
        DiscountPercent = p.DiscountPercent,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        CreatedAt = p.CreatedAt
    })
    .ToListAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}