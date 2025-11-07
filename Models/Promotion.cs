using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.Models
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string PromotionCode { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}