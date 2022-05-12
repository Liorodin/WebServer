namespace WebServer.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string from { get; set; }

        public string type { get; set; }

        public string content { get; set; }

        public DateTime time { get; set; }
    }
}
