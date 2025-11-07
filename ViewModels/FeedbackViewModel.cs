using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.ViewModels
{
    public class FeedbackViewModel
    {
        public int FeedbackId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đặt phòng.")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nhận xét.")]
        [StringLength(1000, ErrorMessage = "Nhận xét không được vượt quá 1000 ký tự.")]
        public string Comments { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}