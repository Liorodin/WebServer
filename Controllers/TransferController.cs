using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebServer.Data;
using WebServer.Models;
using Message = WebServer.Models.Message;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : Controller
    {
        private readonly WebServerContext _context;
        private IDictionary<string, string> _connections;

        public TransferController(WebServerContext context, IDictionary<string, string> connections)
        {
            _context = context;
            _connections = connections;
        }

        public class TransferRequest
        {
            public string? From { get; set; }
            public string? To { get; set; }
            public string? Content { get; set; }
        }

        private void sendFirebaseMessage(String from, String token, String content)
        {
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Data = new Dictionary<string, string>() { { "myData", "1337" } },
                Token = token,
                Notification = new Notification()
                {
                    Title = "New message from " + from,
                    Body = content
                }
            };
            FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        [HttpPost]
        public async Task<IActionResult> PostTransfer([Bind("From, To, Content")] TransferRequest request)
        {
            User reciever = _context.User.Include(x => x.Chats).FirstOrDefault(y => y.Username == request.To);
            if (reciever == null) return BadRequest();
            Chat findChat = null;
            foreach (Chat chat in reciever.Chats)
            {
                Chat currentChat = await _context.Chat.Include(x => x.Contact).Include(z => z.Messages).FirstOrDefaultAsync(y => y.Id == chat.Id);
                if (currentChat.Contact.Username == request.From)
                {
                    findChat = currentChat;
                    break;
                }
            }
            if (findChat == null) return BadRequest();
            Message message = new Message();
            message.Content = request.Content;
            message.from = request.From;
            message.To = request.To;
            message.Time = DateTime.Now;
            message.Type = "text";
            message.ChatID = findChat.Id;
            message.Chat = findChat;
            _context.Message.Add(message);
            findChat.Messages.Add(message);
            await _context.SaveChangesAsync();
            if (_connections.ContainsKey(request.To))
            {
                sendFirebaseMessage(request.From, _connections[request.To], request.Content);
            }
            return Created("", request);
        }
    }
}

