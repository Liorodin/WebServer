using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Contacts
    {
        [Key]
        public string Username { get; set; }

        public ICollection<User> Users { get; set; }

   }
}
