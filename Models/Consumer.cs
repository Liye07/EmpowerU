using EmpowerU.Models;
using System.ComponentModel.DataAnnotations;

public class Consumer : User
{
    [Required]
    [StringLength(45)]
    public string Surname { get; set; }

    public List<string> PreferredCategories { get; set; } 

    public virtual ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();
}