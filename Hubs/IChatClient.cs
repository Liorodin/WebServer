
namespace Chatty.Api.Hubs.Clients
{
    public class ChatMessage
    {
        public string Content { get; set; }
        public string From { get; set; }
    }

    public interface IChatClient
    {
        Task ReceiveMessage(ChatMessage message);
    }
}