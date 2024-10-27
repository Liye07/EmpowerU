using EmpowerU.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

public class Business : User
{
    [StringLength(int.MaxValue)]
    public string? Description { get; set; }

    public int? LocationID { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? Rating { get; set; }

    [Required(ErrorMessage = "Business Category is required.")]
    public List<string> BusinessCategory { get; set; }

    [ForeignKey(nameof(LocationID))]
    public LocationService LocationService { get; set; }

    public ICollection<Review>? Reviews { get; set; } // Updated to ICollection

    public virtual ICollection<Service>? Services { get; set; }

}