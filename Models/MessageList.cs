namespace WebServer.Models
{
    public class MessageList
    {
        public int Id { get; set; }

        public List<User> Users { get; set; }

        public List<Message> Messages { get; set; }
    }
}
