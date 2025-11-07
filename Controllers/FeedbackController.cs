using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Models;
using BookingHomestay.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookingHomestay.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Feedback
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Customer)
                .ToListAsync();
            return View(feedbacks);
        }

        // GET: /Feedback/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Feedback/Create
     [HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public async Task<IActionResult> Create(Feedback feedback)
{
    if (ModelState.IsValid)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
        {
            TempData["ErrorMessage"] = "Bạn cần đăng nhập để gửi phản hồi.";
            return RedirectToAction(nameof(Index));
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == userEmail);
        if (customer == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng.";
            return RedirectToAction(nameof(Index));
        }

        feedback.CustomerId = customer.CustomerId;
        feedback.FeedbackDate = DateTime.Now;
        _context.Add(feedback);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Phản hồi đã được gửi thành công!";
        return RedirectToAction(nameof(Index));
    }
    return View(feedback);
}
        // GET: /Feedback/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        // POST: /Feedback/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Feedback feedback)
        {
            if (id != feedback.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Phản hồi đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.FeedbackId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(feedback);
        }

        // GET: /Feedback/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Customer)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        // POST: /Feedback/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Phản hồi đã được xóa thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.FeedbackId == id);
        }
    }
}