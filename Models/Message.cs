using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Message
    {
        public int Id { get; set; }

        public User From { get; set; }

        public string Type { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Time { get; set; }
    }
}
