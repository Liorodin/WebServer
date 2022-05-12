using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Contacts
    {
        [Key]
        public string username { get; set; }

        public List<User>? userList { get; set; }

   }
}
