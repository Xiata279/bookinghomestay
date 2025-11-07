using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using BookingHomestay.Data;
using BookingHomestay.Models;
using BookingHomestay.ViewModels;
using BCrypt.Net;


namespace BookingHomestay.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Dữ liệu đăng nhập không hợp lệ.");
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                    return View(model);
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.CustomerId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ?? "User"),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Đăng nhập thất bại: {ex.Message}");
                return View(model);
            }
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Dữ liệu đăng ký không hợp lệ.");
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (await _context.Customers.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower()))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã được sử dụng.");
                    return View(model);
                }

                if (await _context.Customers.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }

                var user = new Customer
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Phone = model.Phone,
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Customers.Add(user);
                await _context.SaveChangesAsync();

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.CustomerId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ?? "User"),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Đăng ký thất bại: {ex.Message}");
                return View(model);
            }
        }

        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Đăng xuất thất bại: {ex.Message}";
            }
            return RedirectToAction("Index", "Home");
        }
    }
}