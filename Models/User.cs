using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; }

        [Required]
        public string Nickname { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? Picture { get; set; }

        public ICollection<Chat>? Chats { get; set; }
    }
}
