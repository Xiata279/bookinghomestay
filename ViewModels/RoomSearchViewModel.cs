using System.Collections.Generic;

namespace BookingHomestay.ViewModels
{
    public class RoomSearchViewModel
    {
        public string Location { get; set; } // Giữ kiểu string
        public string RoomType { get; set; }
        public string Status { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortOrder { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalRooms { get; set; }
        public List<RoomViewModel> Rooms { get; set; } = new List<RoomViewModel>();
    }
}