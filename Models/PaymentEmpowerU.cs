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
        public int AppointmentID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [StringLength(10)]
        public string PaymentStatus { get; set; }

        // New field to store Stripe PaymentIntent ID
        [StringLength(255)]
        public string PaymentIntentId { get; set; }

        [StringLength(255)]
        public string Currency { get; set; } = "USD"; // Default currency

        [ForeignKey(nameof(ConsumerID))]
        public Consumer Consumer { get; set; }

        [ForeignKey(nameof(BusinessID))]
        public Business Business { get; set; }

        [ForeignKey(nameof(AppointmentID))]
        public Appointment Appointment { get; set; }
    }
}
