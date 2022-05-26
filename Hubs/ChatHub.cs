using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using static WebServer.Controllers.ContactsController;

internal class ChatHub : Hub
{
    public async Task SendMessage(TempMessage message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}