using EmpowerU.Models;
using System.ComponentModel.DataAnnotations;

public class Consumer : User
{
    [Required]
    [StringLength(45)]
    public string Surname { get; set; }

    public string[] PreferredCategories { get; set; } // Make sure this property exists
}