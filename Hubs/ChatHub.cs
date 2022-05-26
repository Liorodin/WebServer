using Chatty.Api.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

internal class ChatHub : Hub<IChatClient>
{

    public async Task SendMessage(ChatMessage message)
    {
        await Clients.All.ReceiveMessage(message);
    }
}