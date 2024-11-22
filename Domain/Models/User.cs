// Domain/Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Login { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [MaxLength(16)]
        public string BankCardData { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int AddressId { get; set; }

        // Добавляем свойство PasswordHash
        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string RecoveryKeyword { get; set; }

        // Навигационные свойства (если используются)
        public Role Role { get; set; }
        public Address Address { get; set; }
    }
}
