using BookingHomestay.Data;
using BookingHomestay.Models;
using BookingHomestay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookingHomestay.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromotionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Promotions/Index
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageSize = 10;
            var promotionsQuery = _context.Promotions.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                promotionsQuery = promotionsQuery.Where(p => p.PromotionCode.Contains(search) || (p.Description != null && p.Description.Contains(search)));
            }

            var totalPromotions = await promotionsQuery.CountAsync();
            var promotions = await promotionsQuery
                .OrderBy(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling(totalPromotions / (double)pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["Search"] = search;

            return View(promotions);
        }

        // GET: Promotions/Create
        public IActionResult Create()
        {
            return View(new PromotionViewModel());
        }

        // POST: Promotions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromotionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var promotion = new Promotion
                {
                    PromotionCode = model.PromotionCode,
                    Description = model.Description,
                    DiscountPercent = model.DiscountPercent,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CreatedAt = DateTime.Now
                };

                _context.Promotions.Add(promotion);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm khuyến mãi thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Promotions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            var model = new PromotionViewModel
            {
                PromotionId = promotion.PromotionId,
                PromotionCode = promotion.PromotionCode,
                Description = promotion.Description,
                DiscountPercent = promotion.DiscountPercent,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                CreatedAt = promotion.CreatedAt
            };

            return View(model);
        }

        // POST: Promotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PromotionViewModel model)
        {
            if (id != model.PromotionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var promotion = await _context.Promotions.FindAsync(id);
                if (promotion == null)
                {
                    return NotFound();
                }

                promotion.PromotionCode = model.PromotionCode;
                promotion.Description = model.Description;
                promotion.DiscountPercent = model.DiscountPercent;
                promotion.StartDate = model.StartDate;
                promotion.EndDate = model.EndDate;

                _context.Update(promotion);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật khuyến mãi thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Promotions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        // POST: Promotions/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa khuyến mãi thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}