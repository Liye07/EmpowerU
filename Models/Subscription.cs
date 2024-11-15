using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerU.Models
{
    public enum SubscriptionType
    {
        Basic,
        Premium,
        Elite
    }

    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubscriptionID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // Change SubscriptionType to be the enum
        [Required]
        public SubscriptionType SubscriptionType { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }
    }
}
