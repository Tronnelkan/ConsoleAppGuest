using System.ComponentModel.DataAnnotations;

namespace CourseProject.Web.Models
{
    public class AddressViewModel
    {
        public int AddressId { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        // Додайте інші необхідні властивості
    }
}
