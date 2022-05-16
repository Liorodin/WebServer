using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string To { get; set; }

        public string Type { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Time { get; set; }

        public int MessageListId { get; set; }

        public MessageList MessageList { get; set; }
    }
}
