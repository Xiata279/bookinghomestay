using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookingHomestay.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BookingHomestay.Services;
using System;
using BookingHomestay.Data;
using System.Security.Claims;

namespace BookingHomestay.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public BookingController(ILogger<BookingController> logger, ApplicationDbContext context, IEmailService emailService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        [AllowAnonymous]
public async Task<IActionResult> Index(string search, int page = 1)
{
    int pageSize = 10;
    var bookingsQuery = _context.Bookings
        .Include(b => b.Room)
        .ThenInclude(r => r.Hotel)
        .Include(b => b.Customer)
        .Include(b => b.Promotion)
        .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
        bookingsQuery = bookingsQuery.Where(b =>
            (b.Customer != null && b.Customer.FullName != null && b.Customer.FullName.Contains(search)) ||
            (b.Room != null && b.Room.RoomName != null && b.Room.RoomName.Contains(search)) ||
            (b.Room != null && b.Room.Hotel != null && b.Room.Hotel.HotelName != null && b.Room.Hotel.HotelName.Contains(search)));
    }

    var totalBookings = await bookingsQuery.CountAsync();
    var bookings = await bookingsQuery
        .OrderByDescending(b => b.CheckInDate)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    ViewData["TotalPages"] = (int)Math.Ceiling(totalBookings / (double)pageSize);
    ViewData["CurrentPage"] = page;
    ViewData["Search"] = search;

    return View(bookings);
}

        [AllowAnonymous]
public async Task<IActionResult> BookRoom(int roomId)
{
    var room = await _context.Rooms
        .Include(r => r.Hotel)
        .FirstOrDefaultAsync(r => r.RoomId == roomId);

    if (room == null)
    {
        return View("Error", new ErrorViewModel { RequestId = "Phòng không tồn tại." });
    }

    var gallery = await _context.Gallery
        .Where(g => g.RoomId == roomId)
        .ToListAsync();

    var amenities = await _context.RoomAmenities
        .Where(a => a.RoomId == roomId)
        .ToListAsync();

    var model = new BookingViewModel
    {
        RoomId = room.RoomId,
        Room = room,
        Gallery = gallery,
        Amenities = amenities
    };

    return View(model);
}

        public async Task<IActionResult> Create()
        {
            var availableRooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Where(r => r.AvailabilityStatus == "Available")
                .ToListAsync();
            ViewData["AvailableRooms"] = new SelectList(availableRooms, "RoomId", "RoomName");
            ViewData["AvailableServices"] = await _context.Services.ToListAsync();
            return View();
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(BookingViewModel model)
{
    if (ModelState.IsValid)
    {
        // Kiểm tra khách hàng tồn tại dựa trên email
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == model.GuestEmail);

        if (customer == null)
        {
            customer = new Customer
            {
                FullName = model.GuestName,
                Email = model.GuestEmail,
                Phone = model.GuestPhone,
                CreatedAt = DateTime.Now
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Cập nhật thông tin nếu cần
            customer.FullName = model.GuestName;
            customer.Phone = model.GuestPhone;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        var room = await _context.Rooms.FindAsync(model.RoomId);
        if (room == null || room.AvailabilityStatus != "Available")
        {
            TempData["ErrorMessage"] = "Phòng không tồn tại hoặc đã được đặt.";
            return RedirectToAction("Index", "Room");
        }

        var booking = new Booking
        {
            RoomId = model.RoomId,
            CustomerId = customer.CustomerId,
            CheckInDate = model.CheckInDate,
            CheckOutDate = model.CheckOutDate,
            TotalPrice = model.TotalPrice,
            Status = "Pending",
            PaymentStatus = "Pending",
            PromotionId = model.SelectedPromotionId,
            BookingDate = DateTime.Now
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        if (model.SelectedServiceIds != null && model.ServiceQuantities != null && model.SelectedServiceIds.Count == model.ServiceQuantities.Count)
        {
            for (int i = 0; i < model.SelectedServiceIds.Count; i++)
            {
                var service = await _context.Services.FindAsync(model.SelectedServiceIds[i]);
                if (service != null)
                {
                    var bookingService = new BookingService
                    {
                        BookingId = booking.BookingId,
                        ServiceId = model.SelectedServiceIds[i],
                        Quantity = model.ServiceQuantities[i],
                        Price = service.Price * model.ServiceQuantities[i]
                    };
                    _context.BookingServices.Add(bookingService);
                }
            }
            await _context.SaveChangesAsync();
        }

        // Gửi email xác nhận
        var subject = "Xác nhận đặt phòng - BookingHomestay";
        var message = $"<h3>Xác nhận đặt phòng</h3>" +
                      $"<p>Cảm ơn bạn đã đặt phòng tại BookingHomestay!</p>" +
                      $"<p><strong>Phòng:</strong> {room.RoomName}</p>" +
                      $"<p><strong>Ngày nhận phòng:</strong> {model.CheckInDate:dd/MM/yyyy}</p>" +
                      $"<p><strong>Ngày trả phòng:</strong> {model.CheckOutDate:dd/MM/yyyy}</p>" +
                      $"<p><strong>Tổng giá:</strong> {model.TotalPrice:N0} đồng</p>" +
                      $"<p>Vui lòng thanh toán để hoàn tất đặt phòng.</p>";
        await _emailService.SendEmailAsync(model.GuestEmail, subject, message);

        TempData["SuccessMessage"] = "Đặt phòng thành công! Vui lòng kiểm tra email để xem chi tiết.";
        return RedirectToAction("Index");
    }

    model.AvailableServices = await _context.Services.ToListAsync();
    model.AvailablePromotions = await _context.Promotions
        .Where(p => p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now)
        .ToListAsync();
    model.Room = await _context.Rooms.FindAsync(model.RoomId) ?? new Room();
    return View(model);
}

        // GET: /Booking/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Include(b => b.Customer)
                .Include(b => b.Promotion)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: /Booking/Cancel/5
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền hủy đặt phòng
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để hủy đặt phòng.";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || (booking.CustomerId.ToString() != userId && !User.IsInRole("Admin")))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền hủy đặt phòng này.";
                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }

        // POST: /Booking/CancelConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền hủy đặt phòng
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để hủy đặt phòng.";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || (booking.CustomerId.ToString() != userId && !User.IsInRole("Admin")))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền hủy đặt phòng này.";
                return RedirectToAction(nameof(Index));
            }

            booking.Status = "Cancelled";
            booking.PaymentStatus = "Refunded";
            _context.Update(booking);

            var cancellation = new Cancellation
            {
                BookingId = booking.BookingId,
                Reason = "Hủy bởi người dùng",
                CancellationDate = DateTime.Now
            };
            _context.Cancellations.Add(cancellation);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hủy đặt phòng thành công!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Navigated to Booking Privacy.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}