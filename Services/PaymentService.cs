using System;
using System.Net.Http;

namespace BookingHomestay.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;

        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateVNPayPaymentUrl(int bookingId, decimal amount, string returnUrl)
        {
            // Logic tích hợp VNPay (giả lập)
            var vnpayUrl = _configuration["VNPay:PaymentUrl"];
            var tmnCode = _configuration["VNPay:TmnCode"];
            var hashSecret = _configuration["VNPay:HashSecret"];
            return vnpayUrl;
        }
    }
}