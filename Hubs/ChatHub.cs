using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

internal class ChatHub : Hub
{
    public async Task SendMessage(Message message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}