using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerU.Models
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConversationID { get; set; }

        public int User1ID { get; set; } // First user in the conversation
        public int User2ID { get; set; } // Second user in the conversation

        public DateTime CreatedDate { get; set; }

        // Navigation property to access all messages within this conversation
        public ICollection<Message> Messages { get; set; }
    }
}
