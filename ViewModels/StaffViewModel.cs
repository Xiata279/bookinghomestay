using BookingHomestay.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.ViewModels
{
    public class StaffViewModel
    {
        public int StaffId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập vai trò.")]
        [StringLength(50, ErrorMessage = "Vai trò không được vượt quá 50 ký tự.")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string? Phone { get; set; }

        public int? HotelId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Hotel? Hotel { get; set; }
    }
}