using BookingHomestay.Data;
using BookingHomestay.Models;
using BookingHomestay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookingHomestay.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageSize = 9; // Hiển thị 9 đánh giá mỗi trang
            var reviewsQuery = _context.Reviews
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                reviewsQuery = reviewsQuery.Where(r =>
                    r.Title.Contains(search) ||
                    r.Room.RoomName.Contains(search) ||
                    (r.Customer != null && r.Customer.FullName.Contains(search))); // Dòng 33: Thêm kiểm tra null cho r.Customer
            }

            var totalReviews = await reviewsQuery.CountAsync();
            var reviews = await reviewsQuery
                .OrderBy(r => r.ReviewDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new ReviewListViewModel
            {
                Reviews = reviews,
                Search = search,
                TotalPages = (int)Math.Ceiling(totalReviews / (double)pageSize),
                CurrentPage = page
            };

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .ThenInclude(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
            {
                return NotFound();
            }

            var model = new ReviewViewModel
            {
                ReviewId = review.ReviewId,
                RoomId = review.RoomId,
                CustomerId = review.CustomerId,
                Rating = review.Rating,
                Title = review.Title,
                Comment = review.Comment,
                Status = review.Status,
                ReviewDate = review.ReviewDate,
                CreatedAt = review.CreatedAt,
                ModifiedAt = review.ModifiedAt,
                Room = review.Room,
                Customer = review.Customer
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Customers"] = _context.Customers.ToList();
            ViewData["Rooms"] = _context.Rooms.ToList();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (review.Rating < 1 || review.Rating > 5)
            {
                ModelState.AddModelError("Rating", "Điểm đánh giá phải từ 1 đến 5.");
            }

            if (ModelState.IsValid)
            {
                review.ReviewDate = DateTime.Now;
                review.CreatedAt = DateTime.Now;
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo đánh giá thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Customers"] = _context.Customers.ToList();
            ViewData["Rooms"] = _context.Rooms.ToList();
            return View(review);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            ViewData["Customers"] = _context.Customers.ToList();
            ViewData["Rooms"] = _context.Rooms.ToList();
            return View(review);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Review review)
        {
            if (id != review.ReviewId)
            {
                return BadRequest();
            }

            if (review.Rating < 1 || review.Rating > 5)
            {
                ModelState.AddModelError("Rating", "Điểm đánh giá phải từ 1 đến 5.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingReview = await _context.Reviews.FindAsync(id);
                    if (existingReview == null)
                    {
                        return NotFound();
                    }

                    existingReview.CustomerId = review.CustomerId;
                    existingReview.RoomId = review.RoomId;
                    existingReview.Rating = review.Rating;
                    existingReview.Comment = review.Comment;
                    existingReview.Title = review.Title;
                    existingReview.Status = review.Status;
                    existingReview.ModifiedAt = DateTime.Now;

                    _context.Reviews.Update(existingReview);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật đánh giá thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Không thể cập nhật đánh giá. Vui lòng thử lại.");
                }
            }

            ViewData["Customers"] = _context.Customers.ToList();
            ViewData["Rooms"] = _context.Rooms.ToList();
            return View(review);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa đánh giá thành công!";
            }

            return RedirectToAction(nameof(Index));
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
                return RedirectToAction("Details", "Room", new { id = model.RoomId });
            }

            if (model.Rating < 1 || model.Rating > 5)
            {
                ModelState.AddModelError("Rating", "Điểm đánh giá phải từ 1 đến 5.");
                ViewData["RoomName"] = room.RoomName;
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                ViewData["RoomName"] = room.RoomName;
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
            return RedirectToAction("Details", "Room", new { id = model.RoomId });
        }
    }
}