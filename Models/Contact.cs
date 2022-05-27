
namespace WebServer.Models
{
    public class Contact
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string Server { get; set; }

        public int ChatId { get; set; }

        public Chat Chat { get; set; }

    }
}

