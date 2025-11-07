using BookingHomestay.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.ViewModels
{
    public class ReviewViewModel
    {
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khách hàng.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm đánh giá.")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Nhận xét không được vượt quá 1000 ký tự.")]
        public string? Comment { get; set; }

        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự.")]
        public string? Title { get; set; }

        [StringLength(255, ErrorMessage = "URL hình ảnh không được vượt quá 255 ký tự.")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái.")]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public int? BookingId { get; set; }

        public DateTime ReviewDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public Customer? Customer { get; set; }
        public Room? Room { get; set; }
        public Booking? Booking { get; set; }
    }
}