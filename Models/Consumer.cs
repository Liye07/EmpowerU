using EmpowerU.Models;
using System.ComponentModel.DataAnnotations;

public class Consumer : User
{
    public Consumer()
    {
        this.Role = "Consumer";  // Automatically sets Role to 'Consumer'
    }

    [Required]
    [StringLength(45)]
    public string Surname { get; set; }

    [StringLength(255)] // Limit string length
    public string PreferredCategories { get; set; }

}
