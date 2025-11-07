using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Models;
using BookingHomestay.ViewModels;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using BookingHomestay.Data;

namespace BookingHomestay.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ApplicationDbContext _context;

        public CustomerController(ILogger<CustomerController> logger, ApplicationDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageSize = 10;
            var customersQuery = _context.Customers
                .Include(c => c.Bookings)
                .ThenInclude(b => b.Room)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                customersQuery = customersQuery.Where(c =>
                    c.FullName.Contains(search) ||
                    c.Email.Contains(search) ||
                    c.Phone.Contains(search));
            }

            var totalCustomers = await customersQuery.CountAsync();
            var customers = await customersQuery
                .OrderBy(c => c.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling(totalCustomers / (double)pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["Search"] = search;

            return View(customers);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Không thể cập nhật thông tin khách hàng. Vui lòng thử lại.");
                }
            }

            return View(customer);
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Navigated to Customer Privacy.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}