using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace EmpowerU.Models
{
    public class User : IdentityUser<int>
    {
        [Required]
        [StringLength(45)]
        public string Name { get; set; }

        [StringLength(45)]
        public string PhoneNo { get; set; }

        public string? Role { get; set; }

        public DateTime? LastLogin { get; set; }

        [NotMapped] // This prevents the password from being mapped to the database
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
