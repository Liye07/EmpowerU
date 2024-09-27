using EmpowerU.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Business : User
{
    public Business()
    {
        this.Role = "Business";  // Automatically sets Role to 'Business'
    }

    [StringLength(int.MaxValue)] // Allow long descriptions if needed
    public string Description { get; set; }

    [Required]
    public int LocationID { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? Rating { get; set; }

    [ForeignKey(nameof(LocationID))]
    public LocationService LocationService { get; set; }

}
