using EmpowerU.Models;
using System.ComponentModel.DataAnnotations;

public class Consumer : User
{
    [Required(ErrorMessage = "Surname is required.")]
    [StringLength(45)]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Preferred Category is required.")]
    [StringLength(30)]
    public string PreferredCategories { get; set; } 

    public virtual ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();
}