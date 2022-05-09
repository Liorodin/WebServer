using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string feedback { get; set; }

        public int rating { get; set; }
    }
}
