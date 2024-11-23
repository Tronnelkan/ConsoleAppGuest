using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        // Навігаційні властивості
        public ICollection<User> Users { get; set; }

        // Властивість для відображення повної адреси
        [NotMapped]
        public string FullAddress => $"{Street}, {City}, {Country}";
    }
}
