
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using static WebServer.Controllers.ContactsController;

namespace Chatty.Api.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(TempMessage message);
    }
}