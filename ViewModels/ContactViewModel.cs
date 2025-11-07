using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]                                                                                                
        public string Email { get; set; } = string.Empty;                                                                                                                                                                                                                                                                                              /* Luanem copy con cặc */

        [Required(ErrorMessage = "Vui lòng nhập tin nhắn.")]
        [StringLength(1000, ErrorMessage = "Tin nhắn không được vượt quá 1000 ký tự.")]
        public string Message { get; set; } = string.Empty;
    }
}