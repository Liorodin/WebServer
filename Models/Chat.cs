using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        public Contact Contact { get; set; }

        public ICollection<Message> Messages { get; set; }
     
    }
}
