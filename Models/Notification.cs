using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmpowerU.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public string NotificationContent { get; set; }

        [Required]
        public bool IsRead { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }
    }
}
