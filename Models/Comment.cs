using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Feedback { get; set; }

        [RegularExpression("^[1-5]$")]
        public int Rating { get; set; }

        public DateTime Time { get; set; }
    }
}
