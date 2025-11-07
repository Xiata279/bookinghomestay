using BookingHomestay.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace BookingHomestay.ViewModels
{
    public class BookingViewModel
    {
        [Required]
        public int RoomId { get; set; }

        public Room Room { get; set; } = null!;                                                                                                                                                                                                                                                            /* Luanem copy con cặc */

        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng.")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng.")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        public string GuestEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression("0[0-9]{9}", ErrorMessage = "Số điện thoại phải có 10 số và bắt đầu bằng 0")]
        public string GuestPhone { get; set; } = string.Empty;

        public int? SelectedPromotionId { get; set; }

        public List<int>? SelectedServiceIds { get; set; }
        public List<int>? ServiceQuantities { get; set; }

        public List<Service>? AvailableServices { get; set; }
        public List<Promotion>? AvailablePromotions { get; set; }

        public List<BookedDateViewModel> BookedDates { get; set; }

        [NotMapped]
        public int TotalNights => (CheckOutDate - CheckInDate).Days > 0 ? (CheckOutDate - CheckInDate).Days : 0;

        [NotMapped]
        public decimal BasePrice => TotalNights * (Room?.Price ?? 0);

        [NotMapped]
        public decimal ServicePrice
        {
            get
            {
                if (SelectedServiceIds == null || ServiceQuantities == null || AvailableServices == null || SelectedServiceIds.Count != ServiceQuantities.Count)
                    return 0;

                decimal total = 0;
                for (int i = 0; i < SelectedServiceIds.Count; i++)
                {
                    var service = AvailableServices.FirstOrDefault(s => s.ServiceId == SelectedServiceIds[i]);
                    if (service != null)
                    {
                        total += service.Price * ServiceQuantities[i];
                    }
                }
                return total;
            }
        }

        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                decimal total = BasePrice + ServicePrice;

                if (SelectedPromotionId.HasValue && AvailablePromotions != null)
                {
                    var promotion = AvailablePromotions.FirstOrDefault(p => p.PromotionId == SelectedPromotionId);
                    if (promotion != null && promotion.StartDate <= DateTime.Now && promotion.EndDate >= DateTime.Now)
                    {
                        total -= total * (promotion.DiscountPercent / 100);
                    }
                }

                return total;
            }
        }
    }
}