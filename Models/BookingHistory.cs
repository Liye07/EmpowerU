using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerU.Models
{
    public class BookingHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingHistoryID { get; set; }

        [Required]
        public int AppointmentID { get; set; }

        [Required]
        public DateTime ChangeDateTime { get; set; }

        public string ChangeDetails { get; set; }

        [ForeignKey(nameof(AppointmentID))]
        public Appointment Appointment { get; set; }
    }
}
