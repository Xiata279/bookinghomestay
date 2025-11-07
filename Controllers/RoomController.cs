using BookingHomestay.Data;
using BookingHomestay.Models;
using BookingHomestay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;
using Microsoft.AspNetCore.Http;

namespace BookingHomestay.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RoomController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string location, string roomType, string status, int page = 1)
        {
            int pageSize = 9; // Số phòng mỗi trang
            var roomsQuery = _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Gallery)
                .Include(r => r.Location)
                .AsQueryable();

            if (!string.IsNullOrEmpty(location))
            {
                roomsQuery = roomsQuery.Where(r => r.Location != null && r.Location.City.Contains(location));
            }

            if (!string.IsNullOrEmpty(roomType))
            {
                roomsQuery = roomsQuery.Where(r => r.RoomType.Contains(roomType));
            }

            if (!string.IsNullOrEmpty(status))
            {
                roomsQuery = roomsQuery.Where(r => r.AvailabilityStatus == status);
            }

            var totalRooms = await roomsQuery.CountAsync();
            var rooms = await roomsQuery
                .OrderBy(r => r.RoomName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                    LocationId = r.LocationId, // Ánh xạ LocationId
                    Location = r.Location, // Giữ Location để hiển thị thông tin
                    CreatedAt = r.CreatedAt,
                    Hotel = r.Hotel,
                    Gallery = r.Gallery.ToList(),
                    Reviews = r.Reviews.ToList()
                })
                .ToListAsync();

            var model = new RoomSearchViewModel
            {
                Location = location,
                RoomType = roomType,
                Status = status,
                Rooms = rooms,
                TotalPages = (int)Math.Ceiling(totalRooms / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize
            };

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Gallery)
                .Include(r => r.Reviews)
                .ThenInclude(r => r.Customer)
                .Include(r => r.Location)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            var model = new RoomViewModel
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                RoomName = room.RoomName,
                RoomType = room.RoomType,
                RoomNumber = room.RoomNumber,
                Price = room.Price,
                AvailabilityStatus = room.AvailabilityStatus,
                Description = room.Description,
                ImageUrl = room.ImageUrl,
                LocationId = room.LocationId, // Ánh xạ LocationId
                Location = room.Location, // Giữ Location để hiển thị thông tin
                CreatedAt = room.CreatedAt,
                Hotel = room.Hotel,
                Gallery = room.Gallery.ToList(),
                Reviews = room.Reviews.ToList()
            };

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> BookRoom(int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkInDate == default || checkOutDate == default)
            {
                return View("Error", new ErrorViewModel { RequestId = "Vui lòng chọn ngày nhận phòng và ngày trả phòng." });
            }

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Phòng không tồn tại." });
            }

            if (checkInDate >= checkOutDate)
            {
                return View("Error", new ErrorViewModel { RequestId = "Ngày nhận phòng phải trước ngày trả phòng." });
            }

            var bookingExists = await _context.Bookings
                .AnyAsync(b => b.RoomId == roomId &&
                               b.CheckInDate < checkOutDate &&
                               b.CheckOutDate > checkInDate);

            if (bookingExists)
            {
                return View("Error", new ErrorViewModel { RequestId = "Phòng này đã được đặt trong khoảng thời gian này." });
            }

            return RedirectToAction("BookRoom", "Booking", new { roomId, checkInDate, checkOutDate });
        }

        [Authorize]
        public IActionResult AddReview(int roomId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var room = _context.Rooms.Find(roomId);
            if (room == null)
            {
                return NotFound("Phòng không tồn tại.");
            }

            var model = new ReviewViewModel
            {
                RoomId = roomId
            };
            ViewData["RoomName"] = room.RoomName;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var customerId = Convert.ToInt32(User.Identity.Name);
            var room = await _context.Rooms.FindAsync(model.RoomId);
            if (room == null)
            {
                return NotFound("Phòng không tồn tại.");
            }

            // Kiểm tra xem khách hàng đã đặt phòng này chưa
            var hasBooking = await _context.Bookings
                .AnyAsync(b => b.RoomId == model.RoomId && b.CustomerId == customerId);
            if (!hasBooking)
            {
                TempData["ErrorMessage"] = "Bạn cần đặt phòng này trước khi đánh giá.";
                return RedirectToAction("Details", new { id = model.RoomId });
            }

            if (model.Rating < 1 || model.Rating > 5)
            {
                ModelState.AddModelError("Rating", "Điểm đánh giá phải từ 1 đến 5.");
                ViewData["RoomName"] = room.RoomName; // Dòng 164: room đã được kiểm tra null ở trên
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                ViewData["RoomName"] = room.RoomName; // Dòng 188: room đã được kiểm tra null ở trên
                return View(model);
            }

            var review = new Review
            {
                RoomId = model.RoomId,
                CustomerId = customerId,
                Rating = model.Rating,
                Comment = model.Comment,
                Title = model.Title,
                ReviewDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                Status = "Pending"
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi, đang chờ duyệt.";
            return RedirectToAction("Details", new { id = model.RoomId });
        }

        [AllowAnonymous]
        public async Task<IActionResult> CompareRooms(List<int> roomIds)
        {
            if (roomIds == null || !roomIds.Any())
            {
                return View("Error", new ErrorViewModel { RequestId = "Vui lòng chọn ít nhất một phòng để so sánh." });
            }

            var rooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Gallery)
                .Include(r => r.Location)
                .Where(r => roomIds.Contains(r.RoomId))
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
                    LocationId = r.LocationId, // Ánh xạ LocationId
                    Location = r.Location, // Giữ Location để hiển thị thông tin
                    CreatedAt = r.CreatedAt,
                    Hotel = r.Hotel,
                    Gallery = r.Gallery.ToList(),
                    Reviews = r.Reviews.ToList()
                })
                .ToListAsync();

            if (!rooms.Any())
            {
                return View("Error", new ErrorViewModel { RequestId = "Không tìm thấy phòng để so sánh." });
            }

            return View(rooms);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Hotels"] = _context.Hotels.ToList();
            ViewData["Locations"] = _context.Locations.ToList(); // Truyền danh sách địa điểm
            return View(new RoomViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var room = new Room
                {
                    HotelId = model.HotelId,
                    RoomName = model.RoomName,
                    RoomType = model.RoomType,
                    RoomNumber = model.RoomNumber,
                    Price = model.Price,
                    AvailabilityStatus = model.AvailabilityStatus,
                    Description = model.Description,
                    LocationId = model.LocationId, // Sử dụng LocationId thay vì Location
                    CreatedAt = DateTime.Now
                };

                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    model.ImageUrl = "/images/" + fileName;
                    room.ImageUrl = model.ImageUrl;
                }

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm phòng thành công!";
                return RedirectToAction("Index");
            }

            ViewData["Hotels"] = await _context.Hotels.ToListAsync();
            ViewData["Locations"] = await _context.Locations.ToListAsync();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Location)
                .FirstOrDefaultAsync(r => r.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            var model = new RoomViewModel
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                RoomName = room.RoomName,
                RoomType = room.RoomType,
                RoomNumber = room.RoomNumber,
                Price = room.Price,
                AvailabilityStatus = room.AvailabilityStatus,
                Description = room.Description,
                ImageUrl = room.ImageUrl,
                LocationId = room.LocationId, // Ánh xạ LocationId
                Location = room.Location, // Giữ Location để hiển thị thông tin
                CreatedAt = room.CreatedAt,
                Hotel = room.Hotel
            };

            ViewData["Hotels"] = _context.Hotels.ToList();
            ViewData["Locations"] = _context.Locations.ToList();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomViewModel model, IFormFile file)
        {
            if (id != model.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var room = await _context.Rooms
                    .Include(r => r.Gallery)
                    .Include(r => r.Location)
                    .FirstOrDefaultAsync(r => r.RoomId == id);

                if (room == null)
                {
                    return NotFound();
                }

                room.HotelId = model.HotelId;
                room.RoomName = model.RoomName;
                room.RoomType = model.RoomType;
                room.RoomNumber = model.RoomNumber;
                room.Price = model.Price;
                room.AvailabilityStatus = model.AvailabilityStatus;
                room.Description = model.Description;
                room.LocationId = model.LocationId; // Sử dụng LocationId thay vì Location

                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var existingRoomImageUrl = "/images/" + fileName;
                    var gallery = await _context.Gallery
                        .FirstOrDefaultAsync(g => g.RoomId == model.RoomId && g.ImageUrl == existingRoomImageUrl);

                    if (gallery != null)
                    {
                        gallery.ImageUrl = existingRoomImageUrl;
                        gallery.UploadedAt = DateTime.Now;
                        _context.Gallery.Update(gallery);
                    }
                    else
                    {
                        var newGallery = new Gallery
                        {
                            RoomId = model.RoomId,
                            HotelId = model.HotelId,
                            ImageUrl = existingRoomImageUrl,
                            Description = "Hình ảnh chính của phòng",
                            UploadedAt = DateTime.Now
                        };
                        _context.Gallery.Add(newGallery);
                    }

                    room.ImageUrl = existingRoomImageUrl;
                }

                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật phòng thành công!";
                return RedirectToAction("Index");
            }

            ViewData["Hotels"] = await _context.Hotels.ToListAsync();
            ViewData["Locations"] = await _context.Locations.ToListAsync();
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Search(RoomSearchViewModel model)
        {
            var roomsQuery = _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Gallery)
                .Include(r => r.Location)
                .AsQueryable();

            if (model.MinPrice.HasValue)
            {
                roomsQuery = roomsQuery.Where(r => r.Price >= model.MinPrice.Value);
            }
            if (model.MaxPrice.HasValue)
            {
                roomsQuery = roomsQuery.Where(r => r.Price <= model.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(model.Location))
            {
                roomsQuery = roomsQuery.Where(r => r.Location != null && r.Location.City.Contains(model.Location));
            }

            if (!string.IsNullOrEmpty(model.RoomType))
            {
                roomsQuery = roomsQuery.Where(r => r.RoomType.Contains(model.RoomType));
            }

            roomsQuery = model.SortOrder switch
            {
                "price_asc" => roomsQuery.OrderBy(r => r.Price),
                "price_desc" => roomsQuery.OrderByDescending(r => r.Price),
                _ => roomsQuery.OrderBy(r => r.RoomName)
            };

            var rooms = await roomsQuery
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
                    LocationId = r.LocationId, // Ánh xạ LocationId
                    Location = r.Location, // Giữ Location để hiển thị thông tin
                    CreatedAt = r.CreatedAt,
                    Hotel = r.Hotel,
                    Gallery = r.Gallery.ToList(),
                    Reviews = r.Reviews.ToList()
                })
                .ToListAsync();

            model.Rooms = rooms;
            model.TotalRooms = rooms.Count();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Location)
                .FirstOrDefaultAsync(r => r.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            var model = new RoomViewModel
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                RoomName = room.RoomName,
                RoomType = room.RoomType,
                RoomNumber = room.RoomNumber,
                Price = room.Price,
                AvailabilityStatus = room.AvailabilityStatus,
                Description = room.Description,
                ImageUrl = room.ImageUrl,
                LocationId = room.LocationId, // Ánh xạ LocationId
                Location = room.Location, // Giữ Location để hiển thị thông tin
                CreatedAt = room.CreatedAt
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Xóa phòng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}