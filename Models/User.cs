using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace EmpowerU.Models
{
    public class User : IdentityUser<int>
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(45)]
        public string Name { get; set; }

        public string? Role { get; set; }

        public DateTime? LastLogin { get; set; }
        public byte[]? ProfilePicture { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [NotMapped] // This prevents ConfirmPassword from being mapped to the database
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
