using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerU.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewID { get; set; }

        [Required]
        public int BusinessID { get; set; }

        [Required]
        public int ConsumerID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int Rating { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(BusinessID))]
        public Business Business { get; set; }

        [ForeignKey(nameof(ConsumerID))]
        public Consumer Consumer { get; set; }




    }
}
