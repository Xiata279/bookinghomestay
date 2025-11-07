using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.ViewModels
{
    public class PromotionViewModel
    {
        public int PromotionId { get; set; }

        [Required(ErrorMessage = "Tên khuyến mãi là bắt buộc")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã khuyến mãi.")]
        [StringLength(50, ErrorMessage = "Mã khuyến mãi không được vượt quá 50 ký tự.")]
        public string PromotionCode { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phần trăm giảm giá.")]
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100.")]
        public decimal DiscountPercent { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}