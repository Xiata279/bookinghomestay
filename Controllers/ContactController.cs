using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Services;
using System.Threading.Tasks;
using BookingHomestay.ViewModels;
using BookingHomestay.Data;

namespace BookingHomestay.Controllers
{
    public class ContactController : Controller
    {
        private readonly EmailService _emailService;

        public ContactController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emailBody = $@"
                <h2>Liên hệ từ khách hàng</h2>
                <p><strong>Họ tên:</strong> {model.Name}</p>
                <p><strong>Email:</strong> {model.Email}</p>
                <p><strong>Tin nhắn:</strong> {model.Message}</p>";

            await _emailService.SendEmailAsync("admin@bookinghomestay.com", "Tin nhắn liên hệ mới", emailBody);

            TempData["SuccessMessage"] = "Tin nhắn của bạn đã được gửi thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}