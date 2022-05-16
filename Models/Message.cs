using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string from { get; set; }

        public string To { get; set; }

        public string Type { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime? Time { get; set; }

        public int ChatID { get; set; }

        public Chat Chat { get; set; }
    }
}
