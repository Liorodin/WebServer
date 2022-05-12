using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Comment
    {
        public int id { get; set; }

        [Required]
        public string? name { get; set; }

        [Required]
        public string? feedback { get; set; }

        [RegularExpression("^[1-5]$")]
        public int rating { get; set; }

        public DateTime time { get; set; }
    }
}
