using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Models;
using BookingHomestay.ViewModels;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using BookingHomestay.Data;

namespace BookingHomestay.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly ILogger<StaffController> _logger;
        private readonly ApplicationDbContext _context;

        public StaffController(ILogger<StaffController> logger, ApplicationDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageSize = 10;
            var staffQuery = _context.Staffs
                .Include(s => s.Hotel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                staffQuery = staffQuery.Where(s =>
                    s.FullName.Contains(search) ||
                    s.Email.Contains(search) ||
                    s.Phone.Contains(search) ||
                    s.Role.Contains(search));
            }

            var totalStaff = await staffQuery.CountAsync();
            var staff = await staffQuery
                .OrderBy(s => s.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling(totalStaff / (double)pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["Search"] = search;

            return View(staff);
        }

        public IActionResult Create()
        {
            ViewData["Hotels"] = _context.Hotels.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                staff.CreatedAt = DateTime.Now;
                _context.Staffs.Add(staff);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Hotels"] = _context.Hotels.ToList();
            return View(staff);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            ViewData["Hotels"] = _context.Hotels.ToList();
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Staff staff)
        {
            if (id != staff.StaffId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingStaff = await _context.Staffs.FindAsync(id);
                    if (existingStaff == null)
                    {
                        return NotFound();
                    }

                    existingStaff.FullName = staff.FullName;
                    existingStaff.Email = staff.Email;
                    existingStaff.Phone = staff.Phone;
                    existingStaff.Role = staff.Role;
                    existingStaff.HotelId = staff.HotelId;

                    _context.Update(existingStaff);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật nhân viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Không thể cập nhật thông tin nhân viên. Vui lòng thử lại.");
                }
            }

            ViewData["Hotels"] = _context.Hotels.ToList();
            return View(staff);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff != null)
            {
                _context.Staffs.Remove(staff);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa nhân viên thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Navigated to Staff Privacy.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}