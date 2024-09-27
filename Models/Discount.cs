using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmpowerU.Models
{
    public class Discount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscountID { get; set; }

        [Required]
        public int BusinessID { get; set; }

        [Required]
        [StringLength(45)]
        public string DiscountCode { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal DiscountAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(BusinessID))]
        public Business Business { get; set; }
    }
}
