using Chatty.Api.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;

internal class ChatHub : Hub<IChatClient>
{

    public async Task SendMessage(ChatMessage message)
    {
        await Clients.All.ReceiveMessage(message);
    }
}