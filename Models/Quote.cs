using System;
using System.ComponentModel.DataAnnotations;

namespace quotingDojo.Models
{
    public abstract class BaseEntity {}
    public class Quote : BaseEntity
    {
        public int ID { get; set; }
        public User user { get; set; }
        [Required]
        [MinLengthAttribute(5)]
        public string Text { get; set; }
        public DateTime created_at { get; set; }
    }
}