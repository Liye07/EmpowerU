using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmpowerU.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageID { get; set; }

        [Required]
        public int SenderID { get; set; }

        [Required]
        public int ReceiverID { get; set; }

        [Required]
        public int ConversationID { get; set; }

        [Required]
        public string MessageContent { get; set; }

        [Required]
        public bool IsRead { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [ForeignKey(nameof(SenderID))]
        public User Sender { get; set; }

        [ForeignKey(nameof(ReceiverID))]
        public User Receiver { get; set; }

        [ForeignKey(nameof(ConversationID))]
        public Conversation Conversation { get; set; }
    }

}
