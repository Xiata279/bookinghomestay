using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Services;
using System.Threading.Tasks;
using BookingHomestay.ViewModels;
using BookingHomestay.Data;
using Microsoft.Extensions.Logging;
using System;

namespace BookingHomestay.Controllers
{
    public class SupportController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<SupportController> _logger;

        public SupportController(IEmailService emailService, ILogger<SupportController> logger)
        {
            _emailService = emailService;
            _logger = logger;
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
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin.";
                return View(model);
            }

            try
            {
                var emailBody = $@"
                    <h2>Yêu cầu hỗ trợ từ khách hàng</h2>
                    <p><strong>Họ tên:</strong> {model.Name}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Tin nhắn:</strong> {model.Message}</p>";

                await _emailService.SendEmailAsync("support@bookinghomestay.com", "Yêu cầu hỗ trợ mới", emailBody);
                TempData["SuccessMessage"] = "Yêu cầu hỗ trợ của bạn đã được gửi thành công!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email hỗ trợ.");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi yêu cầu. Vui lòng thử lại sau.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}