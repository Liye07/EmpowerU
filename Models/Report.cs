using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerU.Models
{
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportID { get; set; }

        [Required]
        public int BusinessID { get; set; }

        [Required]
        [StringLength(90)]
        public string DateRange { get; set; }

        [Required]
        public string Metrics { get; set; }

        [ForeignKey(nameof(BusinessID))]
        public Business Business { get; set; }
    }
}
