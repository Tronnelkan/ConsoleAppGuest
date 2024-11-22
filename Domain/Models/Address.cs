// Domain/Models/Address.cs
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        // Для отображения полного адреса в ComboBox
        public string FullAddress => $"{Street}, {City}, {Country}";

        // Navigation property
        public ICollection<User> Users { get; set; }
    }
}
