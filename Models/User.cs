
using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Nickname { get; set; }

        [Required]
        public string Password { get; set; }

        public string Picture { get; set; }

        public ICollection<Contacts>? Contacts { get; set; }

        public ICollection<MessageList> Conversations { get; set; }
    }
}
