using System.Collections.Generic;

namespace BookingHomestay.ViewModels
{
    public class HomeViewModel
    {
        public List<RoomViewModel> AvailableRooms { get; set; } = new List<RoomViewModel>();
        public List<PromotionViewModel> Promotions { get; set; } = new List<PromotionViewModel>();
    }
}