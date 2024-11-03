using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerU.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentID { get; set; }

        [Required]
        public int BusinessID { get; set; }

        [Required]
        public int ConsumerID { get; set; }

        [Required]
        public int ServiceID { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        [Required]
        public bool Confirmation { get; set; }

        [ForeignKey(nameof(BusinessID))]
        public Business Business { get; set; }

        [ForeignKey(nameof(ConsumerID))]
        public Consumer Consumer { get; set; }


        [ForeignKey(nameof(ServiceID))]
        public Service Service { get; set; }
    }
}
