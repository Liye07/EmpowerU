using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerU.Models
{
    public class LocationService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocationID { get; set; }

        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double Longitude { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }
    }
}
