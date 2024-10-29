using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerU.Models
{
    public class PaymentEmpowerU
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }

        [Required]
        public int ConsumerID { get; set; }

        [Required]
        public int BusinessID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [StringLength(10)]
        public string PaymentStatus { get; set; }

        [ForeignKey(nameof(ConsumerID))]
        public Consumer Consumer { get; set; }

        [ForeignKey(nameof(BusinessID))]
        public Business Business { get; set; }
    }
}
