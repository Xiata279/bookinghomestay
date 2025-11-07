namespace BookingHomestay.Services
{
    public interface IPaymentService
    {
        string CreateVNPayPaymentUrl(int bookingId, decimal amount, string returnUrl);
    }
}