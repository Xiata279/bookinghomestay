using System;
using System.ComponentModel.DataAnnotations;

namespace BookingHomestay.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FeedbackDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? Status { get; set; }

        public virtual Customer Customer { get; set; }
    }
}