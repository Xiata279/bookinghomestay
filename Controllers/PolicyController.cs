using Microsoft.AspNetCore.Mvc;
using BookingHomestay.Data;

namespace BookingHomestay.Controllers
{
    public class PolicyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}