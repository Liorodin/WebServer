
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

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