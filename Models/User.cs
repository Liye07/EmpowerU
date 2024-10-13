using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerU.Models
{
    public abstract class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required]
        [StringLength(45)]
        public string Name { get; set; }

        [Required]
        [StringLength(45)]
        public string Email { get; set; }

        [StringLength(45)]
        public string PhoneNo { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; } // Store hashed passwords

        [Required]
        [StringLength(10)]
        public string Role { get; set; } // Role column to distinguish Consumer and Business

        public DateTime? LastLogin { get; set; }
    }

}
