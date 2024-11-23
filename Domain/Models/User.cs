using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string RecoveryKeyword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BankCardData { get; set; }

        // Навігаційні властивості
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int AddressId { get; set; }
        public Address AddressEntity { get; set; }
    }
}
