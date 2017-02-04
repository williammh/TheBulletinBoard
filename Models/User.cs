using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace quotingDojo.Models
{
    // public abstract class BaseEntity {}
    public class User : BaseEntity
    {
        public User()
        {
            quotes = new List<Quote>();
        }
        public int ID { get; set; }
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public ICollection<Quote> quotes {get; set;}
    }
}