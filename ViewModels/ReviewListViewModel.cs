using System.Collections.Generic;
using BookingHomestay.Models;

namespace BookingHomestay.ViewModels
{
    public class ReviewListViewModel
    {
        public List<Review> Reviews { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Search { get; set; }
    }
}