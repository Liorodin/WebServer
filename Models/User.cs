

using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class User
    {
        [Key]
        public string username { get; set; }

        public string nickname { get; set; }

        public string password { get; set; }

        public string picture { get; set; }
    }
}
